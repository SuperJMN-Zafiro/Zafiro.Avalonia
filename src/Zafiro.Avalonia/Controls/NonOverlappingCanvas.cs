using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Zafiro.Avalonia.Controls;

public class NonOverlappingCanvas : Panel
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly Dictionary<Control, IDisposable> _childSubscriptions = new Dictionary<Control, IDisposable>();

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
        // Primero medimos todos los hijos
        foreach (var child in Children)
        {
            child.Measure(availableSize);
        }

        var size = new Size();
        
        // Calculamos el tamaño necesario
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

        // Recopilamos las posiciones iniciales y tamaños
        foreach (var child in Children)
        {
            var left = Canvas.GetLeft(child);
            var top = Canvas.GetTop(child);

            // Si no tiene Left o Top, lo colocamos en 0,0
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

        // Resolvemos solapamientos
        ResolveOverlaps(positions);

        // Aplicamos las posiciones finales
        foreach (var pos in positions)
        {
            pos.Control.Arrange(new Rect(pos.Left, pos.Top, pos.Width, pos.Height));
        }

        return finalSize;
    }

    private void ResolveOverlaps(List<ControlPosition> positions)
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

                    // Verificamos si hay solapamiento
                    if (IsOverlapping(pos1, pos2))
                    {
                        hasOverlap = true;
                        
                        // Calculamos el vector de separación
                        double dx = (pos2.Left + pos2.Width / 2) - (pos1.Left + pos1.Width / 2);
                        double dy = (pos2.Top + pos2.Height / 2) - (pos1.Top + pos1.Height / 2);
                        
                        // Normalizamos el vector
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance < 0.0001) // Si la distancia es muy pequeña, forzamos una dirección
                        {
                            dx = 1;
                            dy = 0;
                        }
                        else
                        {
                            dx /= distance;
                            dy /= distance;
                        }

                        // Calculamos la distancia de solapamiento
                        double overlapX = (pos1.Width + pos2.Width) / 2 - Math.Abs((pos1.Left + pos1.Width / 2) - (pos2.Left + pos2.Width / 2));
                        double overlapY = (pos1.Height + pos2.Height) / 2 - Math.Abs((pos1.Top + pos1.Height / 2) - (pos2.Top + pos2.Height / 2));
                        double overlap = Math.Min(overlapX, overlapY);

                        // Aplicamos el desplazamiento
                        double moveDistance = overlap / 2 + 1; // Añadimos 1 de margen
                        var newPos1Left = pos1.Left - dx * moveDistance;
                        var newPos1Top = pos1.Top - dy * moveDistance;
                        var newPos2Left = pos2.Left + dx * moveDistance;
                        var newPos2Top = pos2.Top + dy * moveDistance;
                        
                        pos1.Left = newPos1Left;
                        pos1.Top = newPos1Top;
                        pos2.Left = newPos2Left;
                        pos2.Top = newPos2Top;
                    }
                }
            }
        }
        while (hasOverlap && iteration < MaxIterations);
    }

    private bool IsOverlapping(ControlPosition pos1, ControlPosition pos2)
    {
        // Primero verificamos que no sea el mismo control (por referencia)
        if (ReferenceEquals(pos1.Control, pos2.Control))
        {
            return false;
        }

        return !(pos1.Left + pos1.Width <= pos2.Left ||
                 pos2.Left + pos2.Width <= pos1.Left ||
                 pos1.Top + pos1.Height <= pos2.Top ||
                 pos2.Top + pos2.Height <= pos1.Top);
    }

    private class ControlPosition
    {
        public Control Control { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _disposables.Dispose();
    }
}