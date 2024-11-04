using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class DoubleTable : ObjectTable
{
    public DoubleTable(Table<object, object, object> table) : base(table)
    {
        
    }

    public static ObjectTable CreateDouble<T, Q, R>(Table<T,Q, R> table)
    {
        var matrix = new object[table.Rows, table.Columns];

        for (var r = 0; r < table.Rows; r++)
        for (var c = 0; c < table.Columns; c++)
            matrix[r, c] = table.Matrix[r, c];

        var objectTable = new Table<object, object, object>(matrix, table.RowLabels.Cast<object>().ToList(),
            table.ColumnLabels.Cast<object>().ToList());

        return new DoubleTable(objectTable);
    }
}