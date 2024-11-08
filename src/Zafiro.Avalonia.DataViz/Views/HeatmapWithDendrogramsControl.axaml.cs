using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Zafiro.DataAnalysis;

namespace Zafiro.Avalonia.DataViz.Views;

public class HeatmapWithDendrogramsControl : TemplatedControl
{
    public static readonly StyledProperty<IHeatmapWithDendrograms> SourceProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IHeatmapWithDendrograms>(
            nameof(Source));


    public static readonly StyledProperty<IEnumerable<Color>> ColorListProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IEnumerable<Color>>(nameof(ColorList),
            [Colors.Blue, Colors.White, Colors.Red]);

    public static readonly StyledProperty<bool> DisplayLabelsProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, bool>(nameof(DisplayLabels), true);

    public static readonly StyledProperty<IBrush> CellBorderBrushProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IBrush>(
            nameof(CellBorderBrush));

    public static readonly StyledProperty<double> CellBorderThicknessProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, double>(
            nameof(CellBorderThickness));

    public IHeatmapWithDendrograms Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public IEnumerable<Color> ColorList
    {
        get => GetValue(ColorListProperty);
        set => SetValue(ColorListProperty, value);
    }

    public bool DisplayLabels
    {
        get => GetValue(DisplayLabelsProperty);
        set => SetValue(DisplayLabelsProperty, value);
    }

    public IBrush CellBorderBrush
    {
        get => GetValue(CellBorderBrushProperty);
        set => SetValue(CellBorderBrushProperty, value);
    }

    public double CellBorderThickness
    {
        get => GetValue(CellBorderThicknessProperty);
        set => SetValue(CellBorderThicknessProperty, value);
    }
}