using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using ReactiveUI;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class HeatmapControl : TemplatedControl
{
    public static readonly StyledProperty<IBrush> CellBorderBrushProperty = AvaloniaProperty.Register<HeatmapControl, IBrush>(nameof(CellBorderBrush));

    public IBrush CellBorderBrush
    {
        get => GetValue(CellBorderBrushProperty);
        set => SetValue(CellBorderBrushProperty, value);
    }

    public static readonly StyledProperty<double> CellBorderThicknessProperty = AvaloniaProperty.Register<HeatmapControl, double>(
        nameof(CellBorderThickness));

    public double CellBorderThickness
    {
        get => GetValue(CellBorderThicknessProperty);
        set => SetValue(CellBorderThicknessProperty, value);
    }

    public static readonly StyledProperty<ITable> TableProperty =
        AvaloniaProperty.Register<HeatmapControl, ITable>(nameof(Table));

    public static readonly DirectProperty<HeatmapControl, double> MaximumValueProperty =
        AvaloniaProperty.RegisterDirect<HeatmapControl, double>(
            nameof(MaximumValue), o => o.MaximumValue, (o, v) => o.MaximumValue = v);

    public static readonly StyledProperty<IDataTemplate> RowTemplateProperty =
        AvaloniaProperty.Register<HeatmapControl, IDataTemplate>(
            nameof(RowTemplate));

    public IDataTemplate RowTemplate
    {
        get => GetValue(RowTemplateProperty);
        set => SetValue(RowTemplateProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> ColumnTemplateProperty =
        AvaloniaProperty.Register<HeatmapControl, IDataTemplate>(
            nameof(ColumnTemplate));

    public IDataTemplate ColumnTemplate
    {
        get => GetValue(ColumnTemplateProperty);
        set => SetValue(ColumnTemplateProperty, value);
    }

    public static readonly StyledProperty<object> ColumnHeaderContentProperty =
        AvaloniaProperty.Register<HeatmapControl, object>(
            nameof(ColumnHeaderContent));

    public object ColumnHeaderContent
    {
        get => GetValue(ColumnHeaderContentProperty);
        set => SetValue(ColumnHeaderContentProperty, value);
    }

    public static readonly StyledProperty<object> RowHeaderContentProperty =
        AvaloniaProperty.Register<HeatmapControl, object>(
            nameof(RowHeaderContent));

    public object RowHeaderContent
    {
        get => GetValue(RowHeaderContentProperty);
        set => SetValue(RowHeaderContentProperty, value);
    }

    public static readonly StyledProperty<bool> ShowHeadersProperty = AvaloniaProperty.Register<HeatmapControl, bool>(
        nameof(ShowHeaders), true);

    public bool ShowHeaders
    {
        get => GetValue(ShowHeadersProperty);
        set => SetValue(ShowHeadersProperty, value);
    }

    private double maximumValue;

    public HeatmapControl()
    {
        this.WhenAnyValue(x => x.Table)
            .WhereNotNull()
            .Select(x => x.Cells.Select(c => (double) c.Item).Max())
            .BindTo(this, x => x.MaximumValue);
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

    public static readonly StyledProperty<bool> ShowValuesProperty = AvaloniaProperty.Register<HeatmapControl, bool>(
        nameof(ShowValues), true);

    public bool ShowValues
    {
        get => GetValue(ShowValuesProperty);
        set => SetValue(ShowValuesProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<Color>> ColorListProperty = AvaloniaProperty.Register<HeatmapControl, IEnumerable<Color>>(
        nameof(ColorList), [Colors.Blue, Colors.White, Colors.Red]);

    public IEnumerable<Color> ColorList
    {
        get => GetValue(ColorListProperty);
        set => SetValue(ColorListProperty, value);
    }
}