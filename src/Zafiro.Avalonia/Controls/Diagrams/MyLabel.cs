using System.Reactive.Disposables;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public partial class Label : ReactiveObject, INode, IDisposable
{
    public IEdge<INode> Edge { get; }
    private readonly CompositeDisposable disposables = new();

    public Label(IEdge<INode> edge)
    {
        Edge = edge;
        edge.BoundsChanged().Do(rect =>
            {
                var middle = rect.MiddlePoint();
                Left = middle.X;
                Top = middle.Y;
            })
            .Subscribe()
            .DisposeWith(disposables);
    }

    [ReactiveUI.SourceGenerators.Reactive]
    private double left;

    [ReactiveUI.SourceGenerators.Reactive]
    private double top;

    public string Name => "";

    public void Dispose()
    {
        disposables.Dispose();
    }
}