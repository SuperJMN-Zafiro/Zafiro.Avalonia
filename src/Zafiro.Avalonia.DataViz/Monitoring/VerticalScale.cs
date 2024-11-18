using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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

public class VerticalScale : Control
{
    public static readonly StyledProperty<IEnumerable<double>?> ValuesProperty = AvaloniaProperty.Register<VerticalScale, IEnumerable<double>?>(
        nameof(Values));

    public IEnumerable<double>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    private readonly IDisposable collectionChangesSubscription;

    public VerticalScale()
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

    public static readonly StyledProperty<double> XSpacingProperty = AvaloniaProperty.Register<VerticalScale, double>(
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

        return new Size(0, Values.Max());
    }

    public static readonly StyledProperty<double> LineIntervalProperty = AvaloniaProperty.Register<VerticalScale, double>(
        nameof(LineInterval), 10); // Intervalo predeterminado de 10 unidades

    public double LineInterval
    {
        get => GetValue(LineIntervalProperty);
        set => SetValue(LineIntervalProperty, value);
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

        var height = Bounds.Height;
        var width = Bounds.Width;

        // Obtener el factor de escalado efectivo
        var effectiveScale = GetEffectiveScale();
        var scaleX = effectiveScale.X;
        var scaleY = effectiveScale.Y;

        // Ajustar el grosor de las líneas y el tamaño de la fuente
        double adjustedStrokeThickness = 1 / scaleY; // Mantener un grosor constante de 1 unidad
        double adjustedFontSize = 10 / scaleY; // Mantener un tamaño de fuente constante de 10 unidades

        // Dibuja la línea cero
        var zeroY = TransformY(0, minValue, maxValue, height);
        var middlePen = new Pen(Brushes.Blue, adjustedStrokeThickness);
        context.DrawLine(middlePen, new Point(0, zeroY), new Point(width, zeroY));

        // Configura el intervalo y el estilo de las líneas horizontales
        var interval = LineInterval;
        var linePen = new Pen(Brushes.Gray, adjustedStrokeThickness, dashStyle: DashStyle.Dash);

        // Configurar la fuente para las etiquetas
        var typeface = new Typeface("Arial");
        var textBrush = Brushes.Black;

        // Calcula el rango de valores para las líneas
        double startValue = Math.Floor(minValue / interval) * interval;
        double endValue = Math.Ceiling(maxValue / interval) * interval;

        // Dibuja las líneas horizontales y las etiquetas
        for (double value = startValue; value <= endValue; value += interval)
        {
            var y = TransformY(value, minValue, maxValue, height);

            // Dibuja la línea horizontal
            if (value != 0)
            {
                context.DrawLine(linePen, new Point(0, y), new Point(width, y));
            }

            // Crear el texto formateado
            var formattedText = new FormattedText(
                textToFormat: value.ToString(),
                culture: CultureInfo.CurrentCulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: typeface,
                emSize: adjustedFontSize,
                foreground: textBrush);

            // Posicionar la etiqueta a la izquierda de la línea
            var textPosition = new Point(formattedText.Width, y - formattedText.Height / 2);
            //context.DrawText(formattedText, textPosition);
        }
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
    
    private double TransformY(double value, double minValue, double maxValue, double height)
    {
        // Invertimos el eje Y para que los valores mayores estén en la parte superior
        double range = maxValue - minValue;
        if (range == 0)
        {
            return height / 2;
        }
        else
        {
            return height - ((value - minValue) / range) * height;
        }
    }
}