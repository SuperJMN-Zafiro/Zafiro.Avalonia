using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Zafiro.Avalonia.Controls.DataGrid;

public class DataColumn : AvaloniaObject
{
    public static readonly StyledProperty<IDataTemplate?> TemplateProperty = AvaloniaProperty.Register<DataColumn, IDataTemplate?>(
        nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty = AvaloniaProperty.Register<DataColumn, IDataTemplate?>(
        nameof(HeaderTemplate));

    [Content]
    [InheritDataTypeFromItems(nameof(DataGrid.ItemsSource), AncestorType = typeof(DataGrid))]
    public IDataTemplate? CellTemplate
    {
        get => GetValue(TemplateProperty);
        set => SetValue(TemplateProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(DataGrid.ItemsSource), AncestorType = typeof(DataGrid))]
    public IBinding? Binding { get; set; }

    public object? Header { get; set; }

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
}