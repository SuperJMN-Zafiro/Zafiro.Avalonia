using System.Collections.Generic;
using CSharpFunctionalExtensions;
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
            new Section("Tables", new TableViewModel(), Maybe<object>.None),
            new Section("Heatmaps", new HeatmapViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}