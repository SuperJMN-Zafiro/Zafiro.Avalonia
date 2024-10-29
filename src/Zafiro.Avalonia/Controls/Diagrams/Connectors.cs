using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Zafiro.Avalonia.Controls.Diagrams.Drawing;
using Zafiro.Avalonia.Controls.Diagrams.Drawing.LineStrategies;
using Zafiro.Avalonia.Controls.Diagrams.Drawing.RectConnectorStrategies;
using Zafiro.Graphs;

namespace AvaloniaApplication4
{
    public class Connectors : Control
    {
        public static readonly StyledProperty<IConnectorStrategy> ConnectionStyleProperty = AvaloniaProperty.Register<Connectors, IConnectorStrategy>(
            nameof(ConnectionStyle), SLineConnectorStrategy.Instance);

        public IConnectorStrategy ConnectionStyle
        {
            get => GetValue(ConnectionStyleProperty);
            set => SetValue(ConnectionStyleProperty, value);
        }

        public Connectors()
        {
            AffectsRender<Connectors>(EdgesProperty, HostProperty);
        }

        public static readonly StyledProperty<ItemsControl> HostProperty = AvaloniaProperty.Register<Connectors, ItemsControl>(nameof(Host));

        public ItemsControl Host
        {
            get => GetValue(HostProperty);
            set => SetValue(HostProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> EdgesProperty = AvaloniaProperty.Register<Connectors, IEnumerable>(
            nameof(Edges));

        public IEnumerable Edges
        {
            get => GetValue(EdgesProperty);
            set => SetValue(EdgesProperty, value);
        }

        // Clase para almacenar las conexiones de cada rect�ngulo
        private class RectangleConnections
        {
            public Dictionary<Side, List<IEdge<object>>> ConnectionsPerSide = new Dictionary<Side, List<IEdge<object>>>();
            public Dictionary<Side, Dictionary<IEdge<object>, int>> EdgeIndicesPerSide = new Dictionary<Side, Dictionary<IEdge<object>, int>>();

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

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Host == null || Edges == null)
            {
                return;
            }

            var edges = this.Edges.Cast<IEdge<object>>();
            var pen = new Pen(Brushes.Black, 1);

            // Diccionario para almacenar las conexiones de cada rect�ngulo
            var rectangleConnections = new Dictionary<Control, RectangleConnections>();

            // Nuevo diccionario para almacenar los lados asignados a cada arista
            var edgeSides = new Dictionary<IEdge<object>, Tuple<Side, Side>>();

            // Primera pasada: determinar los lados m�s cercanos y recopilar las conexiones
            foreach (var edge in edges)
            {
                var from = Host.ContainerFromItem(edge.From) as Control;
                var to = Host.ContainerFromItem(edge.To) as Control;

                if (from == null || to == null) continue;

                // Asegurarse de que cada rect�ngulo tenga una entrada en el diccionario
                if (!rectangleConnections.ContainsKey(from))
                {
                    rectangleConnections[from] = new RectangleConnections();
                }
                if (!rectangleConnections.ContainsKey(to))
                {
                    rectangleConnections[to] = new RectangleConnections();
                }

                // Obtener los centros de los rect�ngulos
                var fromBounds = from.Bounds;
                var toBounds = to.Bounds;

                var fromCenter = fromBounds.Center;
                var toCenter = toBounds.Center;

                // Calcular las diferencias en X y Y
                var dx = toCenter.X - fromCenter.X;
                var dy = toCenter.Y - fromCenter.Y;

                // Determinar los lados m�s cercanos
                Side fromSide, toSide;

                if (Math.Abs(dx) >= Math.Abs(dy))
                {
                    // Conexi�n horizontal
                    if (dx >= 0)
                    {
                        // 'To' est� a la derecha de 'From'
                        fromSide = Side.Right;
                        toSide = Side.Left;
                    }
                    else
                    {
                        // 'To' est� a la izquierda de 'From'
                        fromSide = Side.Left;
                        toSide = Side.Right;
                    }
                }
                else
                {
                    // Conexi�n vertical
                    if (dy >= 0)
                    {
                        // 'To' est� debajo de 'From'
                        fromSide = Side.Bottom;
                        toSide = Side.Top;
                    }
                    else
                    {
                        // 'To' est� encima de 'From'
                        fromSide = Side.Top;
                        toSide = Side.Bottom;
                    }
                }

                // Agregar la arista a las listas correspondientes
                rectangleConnections[from].ConnectionsPerSide[fromSide].Add(edge);
                rectangleConnections[from].EdgeIndicesPerSide[fromSide][edge] = 0; // �ndice temporal

                rectangleConnections[to].ConnectionsPerSide[toSide].Add(edge);
                rectangleConnections[to].EdgeIndicesPerSide[toSide][edge] = 0; // �ndice temporal

                // Guardar el lado asignado en el diccionario 'edgeSides'
                edgeSides[edge] = new Tuple<Side, Side>(fromSide, toSide);
            }

            // Segunda pasada: ordenar y asignar �ndices a las conexiones
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

                    // Ordenar las conexiones bas�ndose en la posici�n del centro del rect�ngulo conectado
                    edgesOnSide.Sort((e1, e2) =>
                    {
                        var otherControl1 = GetConnectedControl(e1, control);
                        var otherControl2 = GetConnectedControl(e2, control);

                        var otherCenter1 = otherControl1.Bounds.Center;
                        var otherCenter2 = otherControl2.Bounds.Center;

                        if (side == Side.Left || side == Side.Right)
                        {
                            // Ordenar por coordenada Y
                            return otherCenter1.Y.CompareTo(otherCenter2.Y);
                        }
                        else
                        {
                            // Ordenar por coordenada X
                            return otherCenter1.X.CompareTo(otherCenter2.X);
                        }
                    });

                    // Asignar �ndices despu�s de ordenar
                    for (int i = 0; i < edgesOnSide.Count; i++)
                    {
                        connections.EdgeIndicesPerSide[side][edgesOnSide[i]] = i;
                    }
                }
            }

            // Tercera pasada: distribuir los puntos de conexi�n y dibujar las l�neas
            foreach (var edge in edges)
            {
                var from = Host.ContainerFromItem(edge.From) as Control;
                var to = Host.ContainerFromItem(edge.To) as Control;

                if (from == null || to == null) continue;

                var fromBounds = from.Bounds;
                var toBounds = to.Bounds;

                var fromConnections = rectangleConnections[from];
                var toConnections = rectangleConnections[to];

                // Obtener los lados asignados desde el diccionario 'edgeSides'
                var sides = edgeSides[edge];
                var fromSide = sides.Item1;
                var toSide = sides.Item2;

                // Obtener el �ndice y total de conexiones en cada lado
                int fromIndex = fromConnections.EdgeIndicesPerSide[fromSide][edge];
                int fromTotal = fromConnections.ConnectionsPerSide[fromSide].Count;

                int toIndex = toConnections.EdgeIndicesPerSide[toSide][edge];
                int toTotal = toConnections.ConnectionsPerSide[toSide].Count;

                // Calcular el punto de conexi�n en 'from'
                Point fromPoint = GetConnectionPoint(fromBounds, fromSide, fromIndex, fromTotal);

                // Calcular el punto de conexi�n en 'to'
                Point toPoint = GetConnectionPoint(toBounds, toSide, toIndex, toTotal);

                // Dibujar la l�nea
                ConnectionStyle.Draw(context, pen, fromPoint, fromSide, toPoint, toSide, false, true);
            }
        }

        private Control GetConnectedControl(IEdge<object> edge, Control currentControl)
        {
            var from = Host.ContainerFromItem(edge.From) as Control;
            var to = Host.ContainerFromItem(edge.To) as Control;

            if (from == currentControl)
            {
                return to;
            }
            else
            {
                return from;
            }
        }

        private Point GetConnectionPoint(Rect bounds, Side side, int index, int totalConnections)
        {
            double x = 0;
            double y = 0;

            if (side == Side.Left || side == Side.Right)
            {
                // Lado vertical, distribuir en Y
                double offset = (index + 1) * bounds.Height / (totalConnections + 1);
                y = bounds.Top + offset;
                x = (side == Side.Left) ? bounds.Left : bounds.Right;
            }
            else // Top o Bottom
            {
                // Lado horizontal, distribuir en X
                double offset = (index + 1) * bounds.Width / (totalConnections + 1);
                x = bounds.Left + offset;
                y = (side == Side.Top) ? bounds.Top : bounds.Bottom;
            }

            return new Point(x, y);
        }
    }
}
