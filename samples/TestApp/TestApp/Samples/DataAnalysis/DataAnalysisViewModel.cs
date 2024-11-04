using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.DataAnalysis.Heatmap;
using Zafiro.UI;

namespace TestApp.Samples.DataAnalysis;

public class DataAnalysisViewModel
{
    public DataAnalysisViewModel()
    {
        Sections = new List<Section>()
        {
            new Section("Heatmaps", new HeatmapViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}