using System.Collections;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.DataGrid;

public class DataRow : TemplatedControl
{
    public DataRow(object data, IEnumerable<DataColumn> columns)
    {
        properties = columns.Select((column, i) => new Cell(data, column, i));
    }

    private IEnumerable<Cell> properties;

    public static readonly DirectProperty<DataRow, IEnumerable<Cell>> PropertiesProperty = AvaloniaProperty.RegisterDirect<DataRow, IEnumerable<Cell>>(
        nameof(Properties), o => o.Properties, (o, v) => o.Properties = v);

    public IEnumerable<Cell> Properties
    {
        get => properties;
        set => SetAndRaise(PropertiesProperty, ref properties, value);
    }
}