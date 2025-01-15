using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Zafiro.Avalonia.Controls.SuperCanvas;

public class SuperCanvas : Panel
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly Dictionary<Control, IDisposable> childSubscriptions = new Dictionary<Control, IDisposable>();

    public static readonly DirectProperty<SuperCanvas, LayoutManagerCollection> LayoutersProperty =
        AvaloniaProperty.RegisterDirect<SuperCanvas, LayoutManagerCollection>(
            nameof(Layouters),
            o => o.Layouters);

    private readonly LayoutManagerCollection layouters;

    public SuperCanvas()
    {
        layouters = new LayoutManagerCollection();
    }

    public LayoutManagerCollection Layouters => layouters;

    protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
            
        if (e.OldItems != null)
        {
            foreach (Control child in e.OldItems)
            {
                if (childSubscriptions.TryGetValue(child, out var subscription))
                {
                    subscription.Dispose();
                    childSubscriptions.Remove(child);
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

        childSubscriptions[child] = subscription;
        disposables.Add(subscription);
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
        disposables.Dispose();
    }
}