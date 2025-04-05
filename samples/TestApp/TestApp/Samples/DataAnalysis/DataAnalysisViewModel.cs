using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.DataAnalysis.Dendrograms;
using TestApp.Samples.DataAnalysis.Heatmaps;
using TestApp.Samples.DataAnalysis.Monitoring;
using TestApp.Samples.DataAnalysis.Tables;
using Zafiro.UI;

namespace TestApp.Samples.DataAnalysis;

public class DataAnalysisViewModel
{
    public DataAnalysisViewModel()
    {
        Sections = new List<SectionOld>()
        {
            new SectionOld("Table", new TableViewModel(), Maybe<object>.None),
            new SectionOld("Dendrogram", new DendrogramViewModel(), Maybe<object>.None),
            new SectionOld("Dendrogram lines", new DendrogramLinesViewModel(), Maybe<object>.None),
            new SectionOld("Heatmap", new HeatmapViewModel(), Maybe<object>.None),
            new SectionOld("Heatmap with dendrograms", new HeatmapWithDendrogramsViewModel(), Maybe<object>.None),
            new SectionOld("Sampler", new SamplerViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISectionOld> Sections { get; }
}