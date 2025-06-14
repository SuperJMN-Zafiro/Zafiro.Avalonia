using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Design;

public class DesignTable : Table<string, string, double>
{
    private static readonly Table<string, string, double> TableImplementation = DesignData.GetTable();

    public DesignTable() : base(TableImplementation.Matrix, TableImplementation.RowLabels, TableImplementation.ColumnLabels)
    {
    }
}