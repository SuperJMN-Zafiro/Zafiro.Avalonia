using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Templates;
using DynamicData;
using MoreLinq;
using Zafiro.Avalonia.Drawing;
using Zafiro.DataAnalysis.Graphs;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Diagrams;

public partial class ConnectionController : ReactiveObject, IDisposable
{
    private readonly ItemsControl host;
    private readonly IConnectorStrategy connectorStrategy;
    private readonly CompositeDisposable disposables = new();
    private readonly ConnectionLayoutManager layoutManager;

    public ConnectionController(IEnumerable<IEdge<INode>> edges, IDataTemplate template, ItemsControl host, IConnectorStrategy connectorStrategy)
    {
        layoutManager = new ConnectionLayoutManager();
        this.host = host;
        this.connectorStrategy = connectorStrategy;
        edges.AsObservableChangeSet()
            .Transform(edge =>
            {
                var canvasContent = new CanvasContent(PositionUpdated(edge))
                {
                    Content = template.Build(null!),
                    DataContext = edge,
                };

                return canvasContent;
            })
            .Bind(out var l)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposables);

        Labels = l;
    }


    private IObservable<Point> PositionUpdated(IEdge<INode> edge)
    {
        return edge.BoundsChanged().CombineLatest(this.WhenAnyValue(x => x.Labels).WhereNotNull()).SelectMany(a => SetupPosition(edge, a.Second));
    }

    private IObservable<Point> SetupPosition(IEdge<INode> edge, ReadOnlyObservableCollection<CanvasContent> edges)
    {
        var allEdges = edges.Select(x => x.DataContext).Cast<IEdge<object>>();
        return layoutManager.CalculateLayout(allEdges.ToList(), host)
            .Select(l =>
            {
                var found = l.Connections.FirstOrDefault(x => x.Edge == edge);
                
                if (found == null)
                {
                    return new Point();
                }
                
                return connectorStrategy.LabelPosition(found.FromPoint, found.FromSide, found.ToPoint, found.ToSide);
            });
    }

    [ReactiveUI.SourceGenerators.Reactive]
    private ReadOnlyObservableCollection<CanvasContent> labels;

    public void Dispose()
    {
        disposables.Dispose();
    }
}