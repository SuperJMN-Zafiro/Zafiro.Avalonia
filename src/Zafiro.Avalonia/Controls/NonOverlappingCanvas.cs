namespace Zafiro.Avalonia.Controls;

using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

public class NonOverlappingCanvas : Panel
{
    public static readonly AttachedProperty<double> LeftProperty =
        AvaloniaProperty.RegisterAttached<NonOverlappingCanvas, Control, double>("Left", double.NaN);

    public static readonly AttachedProperty<double> TopProperty =
        AvaloniaProperty.RegisterAttached<NonOverlappingCanvas, Control, double>("Top", double.NaN);

    public static readonly AttachedProperty<double> RightProperty =
        AvaloniaProperty.RegisterAttached<NonOverlappingCanvas, Control, double>("Right", double.NaN);

    public static readonly AttachedProperty<double> BottomProperty =
        AvaloniaProperty.RegisterAttached<NonOverlappingCanvas, Control, double>("Bottom", double.NaN);

    // Métodos estáticos estándar para las propiedades
    public static void SetLeft(Control element, double value) => element.SetValue(LeftProperty, value);
    public static double GetLeft(Control element) => element.GetValue(LeftProperty);
    public static void SetTop(Control element, double value) => element.SetValue(TopProperty, value);
    public static double GetTop(Control element) => element.GetValue(TopProperty);
    public static void SetRight(Control element, double value) => element.SetValue(RightProperty, value);
    public static double GetRight(Control element) => element.GetValue(RightProperty);
    public static void SetBottom(Control element, double value) => element.SetValue(BottomProperty, value);
    public static double GetBottom(Control element) => element.GetValue(BottomProperty);

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
            var left = GetLeft(child);
            var top = GetTop(child);
            
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
            var left = GetLeft(child);
            var top = GetTop(child);

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
                        if (distance < 1) distance = 1;
                        dx /= distance;
                        dy /= distance;

                        // Calculamos la distancia de solapamiento
                        double overlapX = (pos1.Width + pos2.Width) / 2 - Math.Abs((pos1.Left + pos1.Width / 2) - (pos2.Left + pos2.Width / 2));
                        double overlapY = (pos1.Height + pos2.Height) / 2 - Math.Abs((pos1.Top + pos1.Height / 2) - (pos2.Top + pos2.Height / 2));
                        double overlap = Math.Min(overlapX, overlapY);

                        // Aplicamos el desplazamiento
                        double moveDistance = overlap / 2 + 1; // Añadimos 1 de margen
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
}