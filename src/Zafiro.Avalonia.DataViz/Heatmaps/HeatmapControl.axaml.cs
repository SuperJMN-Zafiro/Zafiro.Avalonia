using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ReactiveUI;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class HeatmapControl : TemplatedControl
{
    public static readonly StyledProperty<Color> LowColorProperty =
        AvaloniaProperty.Register<HeatmapControl, Color>(nameof(LowColor));

    public static readonly StyledProperty<Color> HighColorProperty =
        AvaloniaProperty.Register<HeatmapControl, Color>(nameof(HighColor));

    public static readonly StyledProperty<ITable> TableProperty =
        AvaloniaProperty.Register<HeatmapControl, ITable>(nameof(Table));

    public static readonly DirectProperty<HeatmapControl, double> MaximumValueProperty =
        AvaloniaProperty.RegisterDirect<HeatmapControl, double>(
            nameof(MaximumValue), o => o.MaximumValue, (o, v) => o.MaximumValue = v);

    private double maximumValue;

    public HeatmapControl()
    {
        this.WhenAnyValue(x => x.Table)
            .WhereNotNull()
            .Select(x => x.Cells.Select(c => (double) c.Item).Max())
            .BindTo(this, x => x.MaximumValue);
    }

    public Color LowColor
    {
        get => GetValue(LowColorProperty);
        set => SetValue(LowColorProperty, value);
    }

    public Color HighColor
    {
        get => GetValue(HighColorProperty);
        set => SetValue(HighColorProperty, value);
    }

    public ITable Table
    {
        get => GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }

    public double MaximumValue
    {
        get => maximumValue;
        set => SetAndRaise(MaximumValueProperty, ref maximumValue, value);
    }
}