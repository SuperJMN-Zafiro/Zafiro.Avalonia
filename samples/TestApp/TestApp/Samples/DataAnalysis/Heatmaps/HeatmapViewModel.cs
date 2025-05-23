using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TestApp.Samples.Adorners;
using Zafiro.Tables;

namespace TestApp.Samples.DataAnalysis.Heatmaps;

[Icon("mdi-fire")]
public class HeatmapViewModel
{
    public HeatmapViewModel()
    {
        Table = GetTable();
    }

    public ITable Table { get; }

    private ITable? GetTable2()
    {
        var lines = File.ReadAllLines("Samples/DataAnalysis/Heatmaps/Synthetic_Heatmap_Data.csv");
        var dataLines = lines.Skip(1);

        var csv = dataLines.Select(dataLine => dataLine.Split(",")).ToList();
        var matrix = new double[csv.Count, csv.Count];
        for (int i = 0; i < csv.Count; i++)
        {
            for (int j = 0; j < csv.Count; j++)
            {
                matrix[i, j] = Convert.ToDouble(csv[i][j], CultureInfo.InvariantCulture);
            }
        }

        var labels = Enumerable.Range(1, csv.Count).Select(i => i.ToString()).ToList();

        return new Table<string, double>(matrix, labels);
    }

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