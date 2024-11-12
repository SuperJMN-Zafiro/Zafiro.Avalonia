using Avalonia;
using Avalonia.Controls;
using Zafiro.Avalonia.DataViz.Heatmaps;

namespace Zafiro.Avalonia.DataViz.Views.HeatmapWithDendrograms.Components;

public partial class ColumnsDendrogram : UserControl
{
    public ColumnsDendrogram()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<HeatmapControl> HeatmapProperty = AvaloniaProperty.Register<ColumnsDendrogram, HeatmapControl>(
        "Heatmap");

    public HeatmapControl Heatmap
    {
        get => GetValue(HeatmapProperty);
        set => SetValue(HeatmapProperty, value);
    }

}