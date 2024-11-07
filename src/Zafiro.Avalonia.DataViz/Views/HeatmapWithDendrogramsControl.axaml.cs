using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Zafiro.DataAnalysis;

namespace Zafiro.Avalonia.DataViz.Views;

public class HeatmapWithDendrogramsControl : TemplatedControl
{
    public static readonly StyledProperty<IHeatmapWithDendrograms> SourceProperty = AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IHeatmapWithDendrograms>(
        nameof(Source));

    public IHeatmapWithDendrograms Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }


    public static readonly StyledProperty<IEnumerable<Color>> ColorListProperty = AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IEnumerable<Color>>(nameof(ColorList), [Colors.Blue, Colors.White, Colors.Red]);

    public IEnumerable<Color> ColorList
    {
        get => GetValue(ColorListProperty);
        set => SetValue(ColorListProperty, value);
    }
}