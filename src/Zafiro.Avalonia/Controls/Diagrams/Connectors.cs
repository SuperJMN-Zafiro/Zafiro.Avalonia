using System.Collections;
using System.Reactive.Disposables;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using Zafiro.Avalonia.Drawing;
using Zafiro.Avalonia.Drawing.RectConnectorStrategies;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class Connectors : Control
{
    public static readonly StyledProperty<IConnectorStrategy> ConnectionStyleProperty =
        AvaloniaProperty.Register<Connectors, IConnectorStrategy>(
            nameof(ConnectionStyle), SLineConnectorStrategy.Instance);

    public static readonly StyledProperty<ItemsControl?> HostProperty =
        AvaloniaProperty.Register<Connectors, ItemsControl?>(nameof(Host));

    public static readonly StyledProperty<IEnumerable?> EdgesProperty =
        AvaloniaProperty.Register<Connectors, IEnumerable?>(nameof(Edges));

    public static readonly StyledProperty<IBrush> StrokeProperty = AvaloniaProperty.Register<Connectors, IBrush>(
        nameof(Stroke), defaultValue: Brushes.Black);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Connectors, double>(
            nameof(StrokeThickness), defaultValue: 1D);

    public static readonly StyledProperty<IDataTemplate?> EdgeLabelTemplateProperty =
        AvaloniaProperty.Register<Connectors, IDataTemplate?>(nameof(EdgeLabelTemplate));

    private readonly ConnectionLayoutManager layoutManager = new();
    private readonly CompositeDisposable disposables = new();
    private Layout? currentLayout;


    public Connectors()
    {
        AffectsRender<Connectors>(EdgesProperty, HostProperty, StrokeThicknessProperty, StrokeProperty);
        InvalidateWhenContainersLocationChanges();
        SetupLayoutSubscription();
    }
    
    private void SetupLayoutSubscription()
    {
        var hostAndEdgesChanged = this.WhenAnyValue(x => x.Host, x => x.Edges)
            .Where(tuple => tuple.Item1 != null && tuple.Item2 != null);

        var containerPositionsChanged = this.WhenAnyValue(x => x.Host)
            .WhereNotNull()
            .SelectMany(host => Observable.Merge(
                host.ContainerOnChanged(Canvas.LeftProperty),
                host.ContainerOnChanged(Canvas.TopProperty)
            ))
            .Select(_ => (Host, Edges)); // Proyecto al mismo tipo que hostAndEdgesChanged

        Observable.Merge(hostAndEdgesChanged, containerPositionsChanged)
            .Where(tuple => tuple.Item1 != null && tuple.Item2 != null)
            .Select(tuple => 
            {
                var (host, edges) = tuple;
                return layoutManager
                    .CalculateLayout(edges.Cast<IEdge<object>>().ToList(), host)
                    .Catch<Layout, Exception>(ex => 
                    {
                        return Observable.Empty<Layout>();
                    });
            })
            .Switch()
            .Subscribe(layout =>
            {
                currentLayout = layout;
                InvalidateVisual();
            })
            .DisposeWith(disposables);
    }



    public IBrush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public IConnectorStrategy ConnectionStyle
    {
        get => GetValue(ConnectionStyleProperty);
        set => SetValue(ConnectionStyleProperty, value);
    }

    public ItemsControl? Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    public IEnumerable? Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public IDataTemplate? EdgeLabelTemplate
    {
        get => GetValue(EdgeLabelTemplateProperty);
        set => SetValue(EdgeLabelTemplateProperty, value);
    }

    private void InvalidateWhenContainersLocationChanges()
    {
        this.WhenAnyValue(x => x.Host)
            .WhereNotNull()
            .Select(x => x.ContainerOnChanged(Canvas.LeftProperty))
            .Switch()
            .Do(_ => InvalidateVisual())
            .Subscribe()
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Host)
            .WhereNotNull()
            .Select(x => x.ContainerOnChanged(Canvas.TopProperty))
            .Switch()
            .Do(_ => InvalidateVisual())
            .Subscribe()
            .DisposeWith(disposables);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }

    public override void Render(DrawingContext context)
    {
        if (Host == null || Edges == null || currentLayout == null) 
            return;

        var pen = new Pen(Stroke, StrokeThickness);
        
        foreach (var connection in currentLayout.Connections)
        {
            ConnectionStyle.Draw(
                context, 
                pen,
                connection.FromPoint,
                connection.FromSide,
                connection.ToPoint,
                connection.ToSide,
                false, 
                true);
        }
    }
}

public record SidePair(Side From, Side To);
public record Connection(IEdge<object> Edge, Point FromPoint, Side FromSide, Point ToPoint, Side ToSide);
public record Layout(IReadOnlyList<Connection> Connections);

public class RectangleConnections
{
    private readonly Dictionary<Side, List<ConnectionDetails>> connectionsBySide = new();
    
    public RectangleConnections()
    {
        foreach (var side in Enum.GetValues<Side>())
        {
            connectionsBySide[side] = new List<ConnectionDetails>();
        }
    }

    public void AddConnection(IEdge<object> edge, Side side) =>
        connectionsBySide[side].Add(new ConnectionDetails(edge, side));

    public IEnumerable<ConnectionDetails> GetConnectionsForSide(Side side) => 
        connectionsBySide[side];

    public void AssignIndices(IEnumerable<IEdge<object>> sortedEdges, Side side)
    {
        var connections = connectionsBySide[side];
        var edgeToIndex = sortedEdges
            .Select((edge, index) => (edge, index))
            .ToDictionary(x => x.edge, x => x.index);

        foreach (var connection in connections)
        {
            connection.Index = edgeToIndex[connection.Edge];
            connection.TotalConnections = connections.Count;
        }
    }

    public ConnectionDetails GetConnectionDetails(IEdge<object> edge) =>
        connectionsBySide.Values
            .SelectMany(x => x)
            .First(x => x.Edge == edge);
}

public class ConnectionDetails
{
    public IEdge<object> Edge { get; }
    public Side Side { get; }
    public int Index { get; set; }
    public int TotalConnections { get; set; }

    public ConnectionDetails(IEdge<object> edge, Side side)
    {
        Edge = edge;
        Side = side;
    }
}

public static class DictionaryExtensions 
{
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dict,
        TKey key,
        Func<TValue> valueFactory) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = valueFactory();
            dict[key] = value;
        }
        return value;
    }
}