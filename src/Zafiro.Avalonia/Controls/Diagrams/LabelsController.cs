using System.Collections.ObjectModel;
using DynamicData;
using Zafiro.DataAnalysis.Graphs;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class LabelsController
{
    private readonly ReadOnlyObservableCollection<Label> labels;

    public LabelsController(IEnumerable<IEdge<INode>> source)
    {
        var changeSet = source.ToObservableChangeSetIfPossible();

        changeSet
            .Transform(edge => new Label(edge))
            .Bind(out labels)
            .DisposeMany()
            .Subscribe();
    }

    public ReadOnlyObservableCollection<Label> Labels => labels;
}