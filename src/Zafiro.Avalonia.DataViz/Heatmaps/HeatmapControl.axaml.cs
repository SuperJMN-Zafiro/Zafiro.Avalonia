using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class HeatmapControl : TemplatedControl
{
    public static readonly StyledProperty<Color> LowColorProperty = AvaloniaProperty.Register<HeatmapControl, Color>(nameof(LowColor));

    public Color LowColor
    {
        get => GetValue(LowColorProperty);
        set => SetValue(LowColorProperty, value);
    }

    public static readonly StyledProperty<Color> HighColorProperty = AvaloniaProperty.Register<HeatmapControl, Color>(nameof(HighColor));

    public Color HighColor
    {
        get => GetValue(HighColorProperty);
        set => SetValue(HighColorProperty, value);
    }

    public static readonly StyledProperty<DoubleTable> TableProperty = AvaloniaProperty.Register<HeatmapControl, DoubleTable>(nameof(Table));

    public DoubleTable Table
    {
        get => GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }
}