using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public static class Mixin
{
    public static IObservable<IChangeSet<T>> ToObservableChangeSetIfPossible<T>(
        this IEnumerable<T> source
    )
        where T : notnull
    {
        if (source is INotifyCollectionChanged)
        {
            return ((dynamic)source).ToObservableChangeSet();
        }

        return source.AsObservableChangeSet();
    }
}

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