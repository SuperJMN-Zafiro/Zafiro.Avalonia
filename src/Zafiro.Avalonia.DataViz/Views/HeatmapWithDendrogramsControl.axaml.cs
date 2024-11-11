using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
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

    public static readonly StyledProperty<IDataTemplate> ColumnLabelTemplateProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IDataTemplate>(
            nameof(ColumnLabelTemplate));

    public static readonly StyledProperty<IDataTemplate> RowLabelTemplateProperty =
        AvaloniaProperty.Register<HeatmapWithDendrogramsControl, IDataTemplate>(
            nameof(RowLabelTemplate));

    public IDataTemplate RowLabelTemplate
    {
        get => GetValue(RowLabelTemplateProperty);
        set => SetValue(RowLabelTemplateProperty, value);
    }

    public IDataTemplate ColumnLabelTemplate
    {
        get => GetValue(ColumnLabelTemplateProperty);
        set => SetValue(ColumnLabelTemplateProperty, value);
    }

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