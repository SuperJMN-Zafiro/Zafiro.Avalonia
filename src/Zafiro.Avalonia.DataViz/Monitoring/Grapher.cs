using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using DynamicData.Binding;
using ReactiveUI;

namespace Zafiro.Avalonia.DataViz.Monitoring;

public class Grapher : Control
{
    public static readonly StyledProperty<IEnumerable<double>?> ValuesProperty = AvaloniaProperty.Register<Grapher, IEnumerable<double>?>(
        nameof(Values));

    public IEnumerable<double>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    private readonly IDisposable collectionChangesSubscription;

    public Grapher()
    {
        var collections = this
            .WhenAnyValue(x => x.Values)
            .Select(x => x as INotifyCollectionChanged)
            .WhereNotNull();

        var changes = collections
            .Select(a => a.ObserveCollectionChanges())
            .Switch();

        collectionChangesSubscription = changes
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(_ =>
            {
                InvalidateVisual();
                InvalidateMeasure();
            })
            .Subscribe();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        collectionChangesSubscription.Dispose();
    }

    public static readonly StyledProperty<double> XSpacingProperty = AvaloniaProperty.Register<Grapher, double>(
        nameof(XSpacing));

    public double XSpacing
    {
        get => GetValue(XSpacingProperty);
        set => SetValue(XSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Values is null || !Values.Any())
        {
            return new Size();
        }

        var width = (Values.Count() - 1) * XSpacing;
        return new Size(width, Values.Max());
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Values is null || !Values.Any())
        {
            return;
        }

        var valuesArray = Values.ToArray();

        double minValue = valuesArray.Min();
        double maxValue = valuesArray.Max();
        double valueRange = maxValue - minValue;

        if (valueRange == 0)
        {
            valueRange = 1; // Evitar división por cero
        }

        var height = Bounds.Height;
        var width = Bounds.Width;

        var yScale = height / valueRange;

        var streamGeometry = new StreamGeometry();
        using (var geometryContext = streamGeometry.Open())
        {
            var startX = 0;
            var startY = height - ((valuesArray[0] - minValue) * yScale);
            var origin = new Point(startX, startY);

            geometryContext.BeginFigure(origin, false);

            for (int i = 1; i < valuesArray.Length; i++)
            {
                var x = i * XSpacing;
                var y = height - ((valuesArray[i] - minValue) * yScale);
                var point = new Point(x, y);
                geometryContext.LineTo(point, true);
            }

            geometryContext.EndFigure(false);
        }

        var scale = GetEffectiveScale();
        
        var pen = new Pen(Brushes.Black, 1 / scale.Y);
        context.DrawGeometry(null, pen, streamGeometry);
    }
    
    private Vector GetEffectiveScale()
    {
        var transform = this.GetTransformedBounds();
        if (transform == null)
            return new Vector(1, 1);

        var matrix = transform.Value.Transform;
        var scaleX = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
        var scaleY = Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22);

        // Evitar divisiones por cero
        scaleX = scaleX == 0 ? 1 : scaleX;
        scaleY = scaleY == 0 ? 1 : scaleY;

        return new Vector(scaleX, scaleY);
    }

}