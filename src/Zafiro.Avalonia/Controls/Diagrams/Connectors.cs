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
        nameof(Stroke), Brushes.Black);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Connectors, double>(
            nameof(StrokeThickness), 1D);

    public static readonly StyledProperty<IDataTemplate?> EdgeLabelTemplateProperty =
        AvaloniaProperty.Register<Connectors, IDataTemplate?>(nameof(EdgeLabelTemplate));

    private readonly CompositeDisposable disposables = new();

    public Connectors()
    {
        AffectsRender<Connectors>(EdgesProperty, HostProperty);

        InvalidateWhenContainersLocationChanges();
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
        base.Render(context);

        if (Host == null || Edges == null)
        {
            return;
        }

        var edges = Edges.Cast<IEdge<object>>().ToList();
        var pen = new Pen(Stroke, StrokeThickness);

        // Diccionario para almacenar las conexiones de cada rectángulo
        var rectangleConnections = new Dictionary<Control, RectangleConnections>();

        // Nuevo diccionario para almacenar los lados asignados a cada arista
        var edgeSides = new Dictionary<IEdge<object>, Tuple<Side, Side>>();

        // Primera pasada: determinar los lados más cercanos y recopilar las conexiones
        foreach (var edge in edges)
        {
            var from = Host.ContainerFromItem(edge.From);
            var to = Host.ContainerFromItem(edge.To);

            if (from == null || to == null)
            {
                continue;
            }

            // Asegurarse de que cada rectángulo tenga una entrada en el diccionario
            if (!rectangleConnections.ContainsKey(from))
            {
                rectangleConnections[from] = new RectangleConnections();
            }

            if (!rectangleConnections.ContainsKey(to))
            {
                rectangleConnections[to] = new RectangleConnections();
            }

            // Obtener los centros de los rectángulos
            var fromBounds = from.Bounds;
            var toBounds = to.Bounds;

            var fromCenter = fromBounds.Center;
            var toCenter = toBounds.Center;

            // Calcular las diferencias en X y Y
            var dx = toCenter.X - fromCenter.X;
            var dy = toCenter.Y - fromCenter.Y;

            // Determinar los lados más cercanos
            Side fromSide, toSide;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                // Conexión horizontal
                if (dx >= 0)
                {
                    // 'To' está a la derecha de 'From'
                    fromSide = Side.Right;
                    toSide = Side.Left;
                }
                else
                {
                    // 'To' está a la izquierda de 'From'
                    fromSide = Side.Left;
                    toSide = Side.Right;
                }
            }
            else
            {
                // Conexión vertical
                if (dy >= 0)
                {
                    // 'To' está debajo de 'From'
                    fromSide = Side.Bottom;
                    toSide = Side.Top;
                }
                else
                {
                    // 'To' está encima de 'From'
                    fromSide = Side.Top;
                    toSide = Side.Bottom;
                }
            }

            // Agregar la arista a las listas correspondientes
            rectangleConnections[from].ConnectionsPerSide[fromSide].Add(edge);
            rectangleConnections[from].EdgeIndicesPerSide[fromSide][edge] = 0; // Índice temporal

            rectangleConnections[to].ConnectionsPerSide[toSide].Add(edge);
            rectangleConnections[to].EdgeIndicesPerSide[toSide][edge] = 0; // Índice temporal

            // Guardar el lado asignado en el diccionario 'edgeSides'
            edgeSides[edge] = new Tuple<Side, Side>(fromSide, toSide);
        }

        // Segunda pasada: ordenar y asignar índices a las conexiones
        foreach (var kvp in rectangleConnections)
        {
            var control = kvp.Key;
            var connections = kvp.Value;
            var bounds = control.Bounds;
            var center = bounds.Center;

            foreach (var sideKvp in connections.ConnectionsPerSide)
            {
                var side = sideKvp.Key;
                var edgesOnSide = sideKvp.Value;

                // Ordenar las conexiones basándose en la posición del centro del rectángulo conectado
                edgesOnSide.Sort((e1, e2) =>
                {
                    var otherControl1 = GetConnectedControl(e1, control);
                    var otherControl2 = GetConnectedControl(e2, control);

                    var otherCenter1 = otherControl1.Bounds.Center;
                    var otherCenter2 = otherControl2.Bounds.Center;

                    if (side == Side.Left || side == Side.Right)
                        // Ordenar por coordenada Y
                    {
                        return otherCenter1.Y.CompareTo(otherCenter2.Y);
                    }

                    // Ordenar por coordenada X
                    return otherCenter1.X.CompareTo(otherCenter2.X);
                });

                // Asignar índices después de ordenar
                for (var i = 0; i < edgesOnSide.Count; i++) connections.EdgeIndicesPerSide[side][edgesOnSide[i]] = i;
            }
        }

        // Tercera pasada: distribuir los puntos de conexión y dibujar las líneas
        foreach (var edge in edges)
        {
            var from = Host.ContainerFromItem(edge.From);
            var to = Host.ContainerFromItem(edge.To);

            if (from == null || to == null)
            {
                continue;
            }

            var fromBounds = from.Bounds;
            var toBounds = to.Bounds;

            var fromConnections = rectangleConnections[from];
            var toConnections = rectangleConnections[to];

            // Obtener los lados asignados desde el diccionario 'edgeSides'
            var sides = edgeSides[edge];
            var fromSide = sides.Item1;
            var toSide = sides.Item2;

            // Obtener el índice y total de conexiones en cada lado
            var fromIndex = fromConnections.EdgeIndicesPerSide[fromSide][edge];
            var fromTotal = fromConnections.ConnectionsPerSide[fromSide].Count;

            var toIndex = toConnections.EdgeIndicesPerSide[toSide][edge];
            var toTotal = toConnections.ConnectionsPerSide[toSide].Count;

            // Calcular el punto de conexión en 'from'
            var fromPoint = GetConnectionPoint(fromBounds, fromSide, fromIndex, fromTotal);

            // Calcular el punto de conexión en 'to'
            var toPoint = GetConnectionPoint(toBounds, toSide, toIndex, toTotal);

            // Dibujar la línea
            ConnectionStyle.Draw(context, pen, fromPoint, fromSide, toPoint, toSide, false, true);
        }
    }

    private Control GetConnectedControl(IEdge<object> edge, Control currentControl)
    {
        var from = Host!.ContainerFromItem(edge.From);
        var to = Host.ContainerFromItem(edge.To);

        if (from == currentControl)
        {
            return to;
        }

        return from;
    }

    private Point GetConnectionPoint(Rect bounds, Side side, int index, int totalConnections)
    {
        double x = 0;
        double y = 0;

        if (side == Side.Left || side == Side.Right)
        {
            // Lado vertical, distribuir en Y
            var offset = (index + 1) * bounds.Height / (totalConnections + 1);
            y = bounds.Top + offset;
            x = side == Side.Left ? bounds.Left : bounds.Right;
        }
        else // Top o Bottom
        {
            // Lado horizontal, distribuir en X
            var offset = (index + 1) * bounds.Width / (totalConnections + 1);
            x = bounds.Left + offset;
            y = side == Side.Top ? bounds.Top : bounds.Bottom;
        }

        return new Point(x, y);
    }

    // Clase para almacenar las conexiones de cada rectángulo
    private class RectangleConnections
    {
        public readonly Dictionary<Side, List<IEdge<object>>> ConnectionsPerSide = new();
        public readonly Dictionary<Side, Dictionary<IEdge<object>, int>> EdgeIndicesPerSide = new();

        public RectangleConnections()
        {
            ConnectionsPerSide[Side.Left] = new List<IEdge<object>>();
            ConnectionsPerSide[Side.Right] = new List<IEdge<object>>();
            ConnectionsPerSide[Side.Top] = new List<IEdge<object>>();
            ConnectionsPerSide[Side.Bottom] = new List<IEdge<object>>();

            EdgeIndicesPerSide[Side.Left] = new Dictionary<IEdge<object>, int>();
            EdgeIndicesPerSide[Side.Right] = new Dictionary<IEdge<object>, int>();
            EdgeIndicesPerSide[Side.Top] = new Dictionary<IEdge<object>, int>();
            EdgeIndicesPerSide[Side.Bottom] = new Dictionary<IEdge<object>, int>();
        }
    }
}