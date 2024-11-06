using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.DataAnalysis.Dendrograms;
using TestApp.Samples.DataAnalysis.Heatmaps;
using TestApp.Samples.DataAnalysis.Tables;
using Zafiro.UI;

namespace TestApp.Samples.DataAnalysis;

public class DataAnalysisViewModel
{
    public DataAnalysisViewModel()
    {
        Sections = new List<Section>()
        {
            new Section("Table", new TableViewModel(), Maybe<object>.None),
            new Section("Dendrogram", new DendrogramViewModel(), Maybe<object>.None),
            new Section("Dendrogram lines", new DendrogramLinesViewModel(), Maybe<object>.None),
            new Section("Heatmap", new HeatmapViewModel(), Maybe<object>.None),
            new Section("Heatmap with dendrograms", new HeatmapWithDendrogramsViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}