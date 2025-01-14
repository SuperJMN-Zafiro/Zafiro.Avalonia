using Zafiro.Avalonia.Drawing;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

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

            layoutConnections.Add(new Connection(edge,
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