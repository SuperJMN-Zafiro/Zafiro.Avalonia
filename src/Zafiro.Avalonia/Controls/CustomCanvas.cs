using System.Collections.Specialized;
using System.Reactive.Disposables;
using Avalonia.Collections;

namespace Zafiro.Avalonia.Controls
{
    public interface ILayoutManager
    {
        void ProcessLayout(List<ControlPosition> positions);
    }

    public class LayoutManagerCollection : AvaloniaList<ILayoutManager>;

    public class ControlPosition
    {
        public Control Control { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class MidpointLayoutManager : AvaloniaObject, ILayoutManager
    {
        public void ProcessLayout(List<ControlPosition> positions)
        {
            foreach (var pos in positions)
            {
                pos.Left -= pos.Width / 2;
                pos.Top -= pos.Height / 2;
            }
        }
    }

    public class NonOverlappingLayoutManager : AvaloniaObject, ILayoutManager
    {
        public void ProcessLayout(List<ControlPosition> positions)
        {
            const int MaxIterations = 100;
            var iteration = 0;
            bool hasOverlap;

            do
            {
                hasOverlap = false;
                iteration++;

                for (int i = 0; i < positions.Count; i++)
                {
                    for (int j = i + 1; j < positions.Count; j++)
                    {
                        var pos1 = positions[i];
                        var pos2 = positions[j];

                        if (IsOverlapping(pos1, pos2))
                        {
                            hasOverlap = true;
                            
                            double dx = (pos2.Left + pos2.Width / 2) - (pos1.Left + pos1.Width / 2);
                            double dy = (pos2.Top + pos2.Height / 2) - (pos1.Top + pos1.Height / 2);
                            
                            double distance = Math.Sqrt(dx * dx + dy * dy);
                            if (distance < 0.0001)
                            {
                                dx = 1;
                                dy = 0;
                            }
                            else
                            {
                                dx /= distance;
                                dy /= distance;
                            }

                            double overlapX = (pos1.Width + pos2.Width) / 2 - 
                                Math.Abs((pos1.Left + pos1.Width / 2) - (pos2.Left + pos2.Width / 2));
                            double overlapY = (pos1.Height + pos2.Height) / 2 - 
                                Math.Abs((pos1.Top + pos1.Height / 2) - (pos2.Top + pos2.Height / 2));
                            double overlap = Math.Min(overlapX, overlapY);

                            double moveDistance = overlap / 2 + 1;
                            pos1.Left -= dx * moveDistance;
                            pos1.Top -= dy * moveDistance;
                            pos2.Left += dx * moveDistance;
                            pos2.Top += dy * moveDistance;
                        }
                    }
                }
            }
            while (hasOverlap && iteration < MaxIterations);
        }

        private bool IsOverlapping(ControlPosition pos1, ControlPosition pos2)
        {
            if (ReferenceEquals(pos1.Control, pos2.Control))
                return false;

            return !(pos1.Left + pos1.Width <= pos2.Left ||
                    pos2.Left + pos2.Width <= pos1.Left ||
                    pos1.Top + pos1.Height <= pos2.Top ||
                    pos2.Top + pos2.Height <= pos1.Top);
        }
    }

    public class CustomCanvas : Panel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Dictionary<Control, IDisposable> _childSubscriptions = new Dictionary<Control, IDisposable>();

        public static readonly DirectProperty<CustomCanvas, LayoutManagerCollection> LayoutersProperty =
            AvaloniaProperty.RegisterDirect<CustomCanvas, LayoutManagerCollection>(
                nameof(Layouters),
                o => o.Layouters);

        private readonly LayoutManagerCollection _layouters;

        public CustomCanvas()
        {
            _layouters = new LayoutManagerCollection();
        }

        public LayoutManagerCollection Layouters => _layouters;

        protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ChildrenChanged(sender, e);
            
            if (e.OldItems != null)
            {
                foreach (Control child in e.OldItems)
                {
                    if (_childSubscriptions.TryGetValue(child, out var subscription))
                    {
                        subscription.Dispose();
                        _childSubscriptions.Remove(child);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (Control child in e.NewItems)
                {
                    ObserveChildProperties(child);
                }
            }
        }

        private void ObserveChildProperties(Control child)
        {
            var subscription = Observable.FromEventPattern<AvaloniaPropertyChangedEventArgs>(
                h => child.PropertyChanged += h,
                h => child.PropertyChanged -= h)
            .Where(pattern => 
                pattern.EventArgs.Property == Canvas.LeftProperty || 
                pattern.EventArgs.Property == Canvas.TopProperty)
            .Subscribe(_ => InvalidateArrange());

            _childSubscriptions[child] = subscription;
            _disposables.Add(subscription);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            var size = new Size();
            
            foreach (var child in Children)
            {
                var left = Canvas.GetLeft(child);
                var top = Canvas.GetTop(child);
                
                if (!double.IsNaN(left))
                {
                    size = size.WithWidth(Math.Max(size.Width, left + child.DesiredSize.Width));
                }
                
                if (!double.IsNaN(top))
                {
                    size = size.WithHeight(Math.Max(size.Height, top + child.DesiredSize.Height));
                }
            }

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0) return finalSize;

            var positions = new List<ControlPosition>();

            foreach (var child in Children)
            {
                var left = Canvas.GetLeft(child);
                var top = Canvas.GetTop(child);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                positions.Add(new ControlPosition
                {
                    Control = child,
                    Left = left,
                    Top = top,
                    Width = child.DesiredSize.Width,
                    Height = child.DesiredSize.Height
                });
            }

            foreach (var layouter in Layouters)
            {
                layouter.ProcessLayout(positions);
            }

            foreach (var pos in positions)
            {
                pos.Control.Arrange(new Rect(pos.Left, pos.Top, pos.Width, pos.Height));
            }

            return finalSize;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _disposables.Dispose();
        }
    }

    // Clases de conveniencia para mantener compatibilidad
    public class NonOverlappingCanvas : CustomCanvas
    {
        public NonOverlappingCanvas()
        {
            Layouters.Add(new NonOverlappingLayoutManager());
        }
    }

    public class MidpointCanvas : CustomCanvas
    {
        public MidpointCanvas()
        {
            Layouters.Add(new MidpointLayoutManager());
        }
    }
}