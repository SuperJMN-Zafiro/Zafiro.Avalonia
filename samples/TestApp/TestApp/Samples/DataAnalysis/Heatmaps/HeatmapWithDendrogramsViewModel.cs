using System.Collections.Generic;
using TestApp.Samples.Adorners;
using Zafiro.DataAnalysis;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace TestApp.Samples.DataAnalysis.Heatmaps;

[Icon("mdi-data-matrix")]
public class HeatmapWithDendrogramsViewModel
{
    public HeatmapWithDendrogramsViewModel()
    {
        Model = HeatmapWithDendrograms.Create(GetTable(), new SingleLinkageClusteringStrategy<string>(), new SingleLinkageClusteringStrategy<string>());
    }

    public IHeatmapWithDendrograms Model { get; }

    public static Table<string, string, double> GetTable()
    {
        double[,] matrix =
        {
            { 45, 30, 20, 15, 25, 35 },
            { 15, 25, 30, 35, 40, 20 },
            { 5, 10, 25, 30, 35, 15 },
            { 10, 15, 30, 25, 20, 10 },
            { 20, 35, 40, 25, 15, 10 }
        };
        IList<string> columns = ["8-10", "10-12", "12-14", "14-16", "16-18", "18-20"];
        IList<string> rows = ["Café Americano", "Capuccino", "Té", "Chocolate", "Latte"];
        var sut = new Table<string, string, double>(matrix, rows, columns);
        return sut;
    }
}