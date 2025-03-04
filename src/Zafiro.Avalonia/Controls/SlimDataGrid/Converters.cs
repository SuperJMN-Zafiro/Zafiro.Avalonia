using Avalonia.Data.Converters;
using MoreLinq;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public static class Converters
{
    public static FuncValueConverter<IEnumerable<Column>, ColumnDefinitions> ColumnDefsConverter { get; } = new(columns =>
    {
        var columnDefinitions = new ColumnDefinitions();

        columns?.ForEach((column, i) =>
        {
            var columnDefinition = new ColumnDefinition(column.Width.Value, column.Width.GridUnitType);
            if (column.Width.GridUnitType == GridUnitType.Auto)
            {
                columnDefinition.SharedSizeGroup = $"Column{i}";
            }
            columnDefinitions.Add(columnDefinition);
        });

        return columnDefinitions;
    });
}