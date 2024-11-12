using Avalonia;
using Avalonia.Controls;
using Zafiro.Avalonia.DataViz.Heatmaps;

namespace Zafiro.Avalonia.DataViz.Views.HeatmapWithDendrograms.Components;

public partial class RowsDendrogram : UserControl
{
    public RowsDendrogram()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<HeatmapControl> HeatmapProperty = AvaloniaProperty.Register<RowsDendrogram, HeatmapControl>(
        nameof(Heatmap));

    public HeatmapControl Heatmap
    {
        get => GetValue(HeatmapProperty);
        set => SetValue(HeatmapProperty, value);
    }
}