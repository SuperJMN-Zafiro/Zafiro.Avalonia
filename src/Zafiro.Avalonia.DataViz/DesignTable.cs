using System.Collections;
using System.Collections.Generic;
using Zafiro.DataModel;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz;

public static class DesignData
{
    public static Table<string, string, double> GetTable()
    {
        double[,] matrix =
        {
            {45, 30, 20, 15, 25, 35},
            {15, 25, 30, 35, 40, 20},
            {5, 10, 25, 30, 35, 15},
            {10, 15, 30, 25, 20, 10},
            {20, 35, 40, 25, 15, 10},
        };

        IList<string> columns = ["8-10", "10-12", "12-14", "14-16", "16-18", "18-20"];
        IList<string> rows = ["Café Americano", "Capuccino", "Té", "Chocolate", "Latte"];
        var sut = new Table<string, string, double>(matrix, rows, columns);
        return sut;
    }
}

public class DesignTable : ITable
{
    private readonly ITable tableImplementation = DesignData.GetTable();

    public object[,] Matrix => tableImplementation.Matrix;

    public IEnumerable RowLabels => tableImplementation.RowLabels;

    public IEnumerable ColumnLabels => tableImplementation.ColumnLabels;

    public int Rows => tableImplementation.Rows;

    public int Columns => tableImplementation.Columns;

    public IEnumerable<ICell> Cells => tableImplementation.Cells;
}