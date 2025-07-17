using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class Row : TemplatedControl
{
    public Row(object data, Columns columns)
    {
        Data = data;
        properties = columns.Select((column, i) => new Cell(data, column, i));
    }

    public object Data { get; }

    private IEnumerable<Cell> properties;

    public static readonly DirectProperty<Row, IEnumerable<Cell>> PropertiesProperty = AvaloniaProperty.RegisterDirect<Row, IEnumerable<Cell>>(
        nameof(Properties), o => o.Properties, (o, v) => o.Properties = v);

    public IEnumerable<Cell> Properties
    {
        get => properties;
        set => SetAndRaise(PropertiesProperty, ref properties, value);
    }
}