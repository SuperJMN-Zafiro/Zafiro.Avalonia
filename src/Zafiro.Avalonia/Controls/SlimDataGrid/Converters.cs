using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class Converters
{
    public static FuncValueConverter<IEnumerable<Column>, ColumnDefinitions> ColumnDefsConverter { get; } = new(o =>
    {
        var columnDefinitions = new ColumnDefinitions();

        foreach (var columnDefinition in o.Select(column => new ColumnDefinition(1, GridUnitType.Star)))
        {
            columnDefinitions.Add(columnDefinition);
        };

        return columnDefinitions;
    });
}