using System.Collections.Generic;
using System.Linq;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.DataAnalysis.Clustering.Untyped;
using Zafiro.Tables;

namespace TestApp.Samples.DataAnalysis.Heatmap;

public class HeatmapViewModel
{
    public HeatmapViewModel()
    {
        var strategy = new SingleLinkageClusteringStrategy<string>();
        var heatmap = Zafiro.DataAnalysis.Heatmap.Create(GetTable(), strategy, strategy);
        Values = heatmap.Table.Items();
        Columns = heatmap.Table.Columns;
        Rows = heatmap.Table.Rows;

        LeftCluster = ClusterNode.Create(heatmap.RowsCluster);
        TopCluster = ClusterNode.Create(heatmap.ColumnsCluster);
    }

    public ClusterNode TopCluster { get; }

    public ClusterNode LeftCluster { get; }

    public IEnumerable<double> Values { get; }

    public int Columns { get; }
    public int Rows { get; }

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

public class ValueViewModel
{
    public double Value { get; }
    public double Normalized { get; }

    public ValueViewModel(double value, double normalized)
    {
        Value = value;
        Normalized = normalized;
    }
}