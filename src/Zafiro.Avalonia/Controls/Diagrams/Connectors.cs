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
    private readonly ConnectionLayoutManager layoutManager = new();
    
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

    private readonly CompositeDisposable disposables = new();

    public Connectors()
    {
        AffectsRender<Connectors>(EdgesProperty, HostProperty, StrokeThicknessProperty, StrokeProperty);
        InvalidateWhenContainersLocationChanges();

        // StrokeThickness = 1;
        // Stroke = Brushes.Black;
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
        if (Host == null || Edges == null) return;

        var edges = Edges.Cast<IEdge<object>>().ToList();
        var pen = new Pen(Stroke, StrokeThickness);
        
        var layout = layoutManager.CalculateLayout(edges, Host);
        foreach (var connection in layout.Connections)
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

public class ConnectionLayoutManager 
{
    public Layout CalculateLayout(IReadOnlyList<IEdge<object>> edges, ItemsControl host)
    {
        var rectangleConnections = GatherConnections(edges, host);
        AssignConnectionIndices(rectangleConnections, host);
        return CreateLayout(edges, host, rectangleConnections);
    }

    private Dictionary<Control, RectangleConnections> GatherConnections(
        IReadOnlyList<IEdge<object>> edges, 
        ItemsControl host)
    {
        var connections = new Dictionary<Control, RectangleConnections>();
        
        foreach (var edge in edges)
        {
            var (from, to) = GetControls(edge, host);
            if (from == null || to == null) continue;

            connections.GetOrAdd(from, () => new RectangleConnections());
            connections.GetOrAdd(to, () => new RectangleConnections());

            var sides = DetermineBestSides(from.Bounds.Center, to.Bounds.Center);
            
            connections[from].AddConnection(edge, sides.From);
            connections[to].AddConnection(edge, sides.To);
        }

        return connections;
    }

    private SidePair DetermineBestSides(Point fromCenter, Point toCenter)
    {
        var dx = toCenter.X - fromCenter.X;
        var dy = toCenter.Y - fromCenter.Y;
        
        return Math.Abs(dx) >= Math.Abs(dy)
            ? new SidePair(dx >= 0 ? Side.Right : Side.Left, dx >= 0 ? Side.Left : Side.Right)
            : new SidePair(dy >= 0 ? Side.Bottom : Side.Top, dy >= 0 ? Side.Top : Side.Bottom);
    }

    private void AssignConnectionIndices(Dictionary<Control, RectangleConnections> connections, ItemsControl host)
    {
        foreach (var (control, rectConnections) in connections)
        {
            foreach (var side in Enum.GetValues<Side>())
            {
                var edgesOnSide = rectConnections.GetConnectionsForSide(side);
                var sortedEdges = SortEdgesByPosition(edgesOnSide, control, host);
                rectConnections.AssignIndices(sortedEdges, side);
            }
        }
    }

    private (Control? from, Control? to) GetControls(IEdge<object> edge, ItemsControl host) => 
        (host.ContainerFromItem(edge.From), host.ContainerFromItem(edge.To));

    private Control GetConnectedControl(IEdge<object> edge, Control currentControl, ItemsControl host)
    {
        var (from, to) = GetControls(edge, host);
        return from == currentControl ? to! : from!;
    }

    private Layout CreateLayout(
        IReadOnlyList<IEdge<object>> edges,
        ItemsControl host,
        Dictionary<Control, RectangleConnections> connections)
    {
        var layoutConnections = new List<Connection>();

        foreach (var edge in edges)
        {
            var (from, to) = GetControls(edge, host);
            if (from == null || to == null) continue;

            var fromConnection = connections[from].GetConnectionDetails(edge);
            var toConnection = connections[to].GetConnectionDetails(edge);

            layoutConnections.Add(new Connection(
                GetConnectionPoint(from.Bounds, fromConnection),
                fromConnection.Side,
                GetConnectionPoint(to.Bounds, toConnection),
                toConnection.Side));
        }

        return new Layout(layoutConnections);
    }

    private IEnumerable<IEdge<object>> SortEdgesByPosition(
        IEnumerable<ConnectionDetails> connections,
        Control sourceControl,
        ItemsControl host)
    {
        return connections
            .OrderBy(c => GetConnectedControl(c.Edge, sourceControl, host).Bounds.Center.Y)
            .Select(c => c.Edge);
    }

    private Point GetConnectionPoint(Rect bounds, ConnectionDetails connection)
    {
        var offset = (connection.Index + 1.0) / (connection.TotalConnections + 1);
        return connection.Side switch
        {
            Side.Left => new Point(bounds.Left, bounds.Top + bounds.Height * offset),
            Side.Right => new Point(bounds.Right, bounds.Top + bounds.Height * offset),
            Side.Top => new Point(bounds.Left + bounds.Width * offset, bounds.Top),
            Side.Bottom => new Point(bounds.Left + bounds.Width * offset, bounds.Bottom),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public record SidePair(Side From, Side To);
public record Connection(Point FromPoint, Side FromSide, Point ToPoint, Side ToSide);
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