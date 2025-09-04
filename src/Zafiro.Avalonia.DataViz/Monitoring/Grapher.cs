using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.ViewLocators;

namespace Zafiro.Avalonia.DataViz.Monitoring;

public class Grapher : Control
{
    public static readonly StyledProperty<IEnumerable<double>?> ValuesProperty = AvaloniaProperty.Register<Grapher, IEnumerable<double>?>(
        nameof(Values));

    public static readonly StyledProperty<double> XSpacingProperty = AvaloniaProperty.Register<Grapher, double>(
        nameof(XSpacing));

    public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<Grapher, double>(
        nameof(StrokeThickness), 1d);

    public static readonly StyledProperty<IBrush> StrokeProperty = AvaloniaProperty.Register<Grapher, IBrush>(
        nameof(Stroke), Brushes.Black);

    public static readonly DirectProperty<Grapher, Vector> EffectiveScaleProperty = AvaloniaProperty.RegisterDirect<Grapher, Vector>(
        nameof(EffectiveScale), o => o.EffectiveScale, (o, v) => o.EffectiveScale = v);

    private readonly CompositeDisposable disposables = new();

    private Vector effectiveScale;

    static Grapher()
    {
        AffectsRender<Grapher>(ValuesProperty, XSpacingProperty, StrokeThicknessProperty, StrokeProperty);
        AffectsMeasure<Grapher>(ValuesProperty, XSpacingProperty);
    }

    public Grapher()
    {
        InvalidateWhenCollectionChanges().DisposeWith(disposables);

        this.EffectiveScale(TimeSpan.FromMilliseconds(500))
            .BindTo(this, x => x.EffectiveScale)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.EffectiveScale)
            .Do(_ => InvalidateVisual())
            .Subscribe();
    }

    public Vector EffectiveScale
    {
        get => effectiveScale;
        private set => SetAndRaise(EffectiveScaleProperty, ref effectiveScale, value);
    }

    public IEnumerable<double>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public double XSpacing
    {
        get => GetValue(XSpacingProperty);
        set => SetValue(XSpacingProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public IBrush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    private IDisposable InvalidateWhenCollectionChanges()
    {
        var collections = this
            .WhenAnyValue(x => x.Values)
            .Select(x => x as INotifyCollectionChanged)
            .WhereNotNull();

        var changes = collections
            .Select(a => a.ObserveCollectionChanges())
            .Switch();

        return changes
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
        disposables.Dispose();
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

        var minValue = valuesArray.Min();
        var maxValue = valuesArray.Max();
        var valueRange = maxValue - minValue;

        if (valueRange == 0)
        {
            valueRange = 1; // Avoid division by zero
        }

        var height = Bounds.Height;

        var yScale = height / valueRange;

        var streamGeometry = new StreamGeometry();
        using (var geometryContext = streamGeometry.Open())
        {
            var startX = 0;
            var startY = height - (valuesArray[0] - minValue) * yScale;
            var origin = new Point(startX, startY);

            geometryContext.BeginFigure(origin, false);

            for (var i = 1; i < valuesArray.Length; i++)
            {
                var x = i * XSpacing;
                var y = height - (valuesArray[i] - minValue) * yScale;
                var point = new Point(x, y);
                geometryContext.LineTo(point, true);
            }

            geometryContext.EndFigure(false);
        }

        var pen = new Pen(Stroke, StrokeThickness / EffectiveScale.Y);
        context.DrawGeometry(null, pen, streamGeometry);
    }
}