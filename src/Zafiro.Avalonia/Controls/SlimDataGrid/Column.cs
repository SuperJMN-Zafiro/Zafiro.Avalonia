using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class Column : AvaloniaObject
{
    public static readonly StyledProperty<IDataTemplate?> TemplateProperty = AvaloniaProperty.Register<Column, IDataTemplate?>(
        nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty = AvaloniaProperty.Register<Column, IDataTemplate?>(
        nameof(HeaderTemplate));

    [Content]
    [InheritDataTypeFromItems(nameof(SlimDataGrid.ItemsSource), AncestorType = typeof(SlimDataGrid))]
    public IDataTemplate? CellTemplate
    {
        get => GetValue(TemplateProperty);
        set => SetValue(TemplateProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(SlimDataGrid.ItemsSource), AncestorType = typeof(SlimDataGrid))]
    public IBinding? Binding { get; set; }

    public object? Header { get; set; }

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
}