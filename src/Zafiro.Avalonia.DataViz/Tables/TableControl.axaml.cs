using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Tables;

public class TableControl : TemplatedControl
{
    public static readonly StyledProperty<ObjectTable> TableProperty =
        AvaloniaProperty.Register<TableControl, ObjectTable>(
            nameof(Table));

    public static readonly StyledProperty<IDataTemplate> CellTemplateProperty =
        AvaloniaProperty.Register<TableControl, IDataTemplate>(
            nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate> RowTemplateProperty =
        AvaloniaProperty.Register<TableControl, IDataTemplate>(
            nameof(RowTemplate));

    public static readonly StyledProperty<IDataTemplate> ColumnTemplateProperty =
        AvaloniaProperty.Register<TableControl, IDataTemplate>(
            nameof(ColumnTemplate));

    public static readonly StyledProperty<IBrush> RowHeaderBackgroundProperty =
        AvaloniaProperty.Register<TableControl, IBrush>(
            nameof(RowHeaderBackground));

    public static readonly StyledProperty<IBrush> ColumnHeaderBackgroundProperty =
        AvaloniaProperty.Register<TableControl, IBrush>(
            nameof(ColumnHeaderBackground));

    public static readonly StyledProperty<bool> ShowHeadersProperty = AvaloniaProperty.Register<TableControl, bool>(
        nameof(ShowHeaders), true);

    public ObjectTable Table
    {
        get => GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }

    public IDataTemplate CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    public IDataTemplate RowTemplate
    {
        get => GetValue(RowTemplateProperty);
        set => SetValue(RowTemplateProperty, value);
    }

    public IDataTemplate ColumnTemplate
    {
        get => GetValue(ColumnTemplateProperty);
        set => SetValue(ColumnTemplateProperty, value);
    }

    public IBrush RowHeaderBackground
    {
        get => GetValue(RowHeaderBackgroundProperty);
        set => SetValue(RowHeaderBackgroundProperty, value);
    }

    public IBrush ColumnHeaderBackground
    {
        get => GetValue(ColumnHeaderBackgroundProperty);
        set => SetValue(ColumnHeaderBackgroundProperty, value);
    }

    public bool ShowHeaders
    {
        get => GetValue(ShowHeadersProperty);
        set => SetValue(ShowHeadersProperty, value);
    }
}