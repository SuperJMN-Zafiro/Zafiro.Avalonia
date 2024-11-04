using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Heatmaps;

public class HeatmapControl : TemplatedControl
{
    public static readonly StyledProperty<Color> LowColorProperty = AvaloniaProperty.Register<HeatmapControl, Color>(
        nameof(LowColor));

    public Color LowColor
    {
        get => GetValue(LowColorProperty);
        set => SetValue(LowColorProperty, value);
    }

    public static readonly StyledProperty<Color> HighColorProperty = AvaloniaProperty.Register<HeatmapControl, Color>(
        nameof(HighColor));

    public Color HighColor
    {
        get => GetValue(HighColorProperty);
        set => SetValue(HighColorProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<double>> ValuesProperty = AvaloniaProperty.Register<HeatmapControl, IEnumerable<double>>(
        nameof(Values));

    public IEnumerable<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public HeatmapControl()
    {
        this.WhenAnyValue(x => x.Values)
            .WhereNotNull()
            .Select(values =>
            {
                var list = values.ToList();
                var max = list.Max();
                return list.Select(v => new ValueModel(v, max));
            })
            .BindTo(this, x => x.ValueModels);
    }

    private IEnumerable<ValueModel> valueModels;

    public static readonly DirectProperty<HeatmapControl, IEnumerable<ValueModel>> ValueModelsProperty = AvaloniaProperty.RegisterDirect<HeatmapControl, IEnumerable<ValueModel>>(
        nameof(ValueModels), o => o.ValueModels, (o, v) => o.ValueModels = v);

    public IEnumerable<ValueModel> ValueModels
    {
        get => valueModels;
        private set => SetAndRaise(ValueModelsProperty, ref valueModels, value);
    }

    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<HeatmapControl, int>(
        nameof(Columns));

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public static readonly StyledProperty<int> RowsProperty = AvaloniaProperty.Register<HeatmapControl, int>(
        nameof(Rows));

    public int Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }
}

public class ValueModel
{
    public double Value { get; }
    public double Max { get; }

    public ValueModel(double value, double max)
    {
        Value = value;
        Max = max;
        Normalized = Value / Max;
    }

    public double Normalized { get; }
}