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

public class YValues : Control
{
    private readonly IDisposable collectionChangesSubscription;

    public static readonly StyledProperty<IEnumerable<double>> ValuesProperty = AvaloniaProperty.Register<YValues, IEnumerable<double>>(
        nameof(Values));

    public IEnumerable<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public YValues()
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
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var values = Values?.ToList();
        
        if (values is null || !values.Any())
        {
            return new Size();
        }
        
        double minValue = values.Min();
        double maxValue = values.Max();
        double interval = LineInterval;
        
        double startValue = Math.Floor(minValue / interval) * interval;
        double endValue = Math.Ceiling(maxValue / interval) * interval;
        
        var width = new[]{ startValue, endValue}.Max(d => FormatText(d.ToString(CultureInfo.InvariantCulture)).Width);
        var height = (values.Max() - values.Min()) * LineInterval;
        
        return new Size(width, height);
    }

    private FormattedText FormatText(string str)
    {
        // Configurar la fuente para las etiquetas
        var typeface = new Typeface("Arial");
        var textBrush = Brushes.Black;
        
        // Obtener el factor de escalado efectivo
        var effectiveScale = GetEffectiveScale();
        var scaleX = effectiveScale.X;
        var scaleY = effectiveScale.Y;

        // Ajustar el grosor de las líneas y el tamaño de la fuente
        double adjustedStrokeThickness = 1 / scaleY; // Mantener un grosor constante de 1 unidad
        double adjustedFontSize = 10 / scaleY; // Mantener un tamaño de fuente constante de 10 unidades
        
        var formattedText = new FormattedText(
            textToFormat: str.ToString(),
            culture: CultureInfo.CurrentCulture,
            flowDirection: FlowDirection.LeftToRight,
            typeface: typeface,
            emSize: adjustedFontSize,
            foreground: textBrush);

        return formattedText;
    }

    private static double TransformY(double value, double minValue, double maxValue, double height)
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

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        collectionChangesSubscription.Dispose();
    }

    public static readonly StyledProperty<double> LineIntervalProperty = AvaloniaProperty.Register<YValues, double>(
        nameof(LineInterval));

    public double LineInterval
    {
        get => GetValue(LineIntervalProperty);
        set => SetValue(LineIntervalProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var values = Values?.ToArray();
        
        if (values is null || !values.Any())
        {
            return;
        }

        double minValue = values.Min();
        double maxValue = values.Max();

        var height = Bounds.Height;
        var width = Bounds.Width;

        // Obtener el factor de escalado efectivo
        var effectiveScale = GetEffectiveScale();
        var scaleX = effectiveScale.X;
        var scaleY = effectiveScale.Y;

        // Ajustar el grosor de las líneas y el tamaño de la fuente

        // Configura el intervalo y el estilo de las líneas horizontales
        var interval = LineInterval;

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
                //context.DrawLine(linePen, new Point(0, y), new Point(width, y));
            }

            // Crear el texto formateado
            var formattedText = FormatText(value.ToString(CultureInfo.InvariantCulture));

            // Posicionar la etiqueta a la izquierda de la línea
            var textPosition = new Point(0, y - formattedText.Height / 2);
            context.DrawText(formattedText, textPosition);
        }
    }
    
    private Vector GetEffectiveScale()
    {
        var visualRoot = VisualRoot as Visual;
        if (visualRoot == null)
            return new Vector(1, 1);

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