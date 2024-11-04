using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz;

public class DoubleCell(int rowIndex, int columnIndex, object rowTag, object columnTag, double value, double max)
{
    public double Normalized { get; } = value / max;
    public int RowIndex { get; } = rowIndex;
    public int ColumnIndex { get; } = columnIndex;
    public object RowTag { get; } = rowTag;
    public object ColumnTag { get; } = columnTag;
    public double Value { get; set; } = value;
}