using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

public class QuadtreeEngine
{
    public QuadtreeEngine(IGraph2D graph)
    {
        Graph = graph;
    }

    public Configuration Configuration { get; } = new();

    public IGraph2D Graph { get; }

    public void Step()
    {
        // Calcula las fuerzas repulsivas usando Quadtree.
        Repel();

        // Calcula las fuerzas atractivas.
        Attract();

        // Actualiza las posiciones de los nodos.
        UpdatePositions();
    }

    private void Repel()
    {
        var root = BuildQuadtree();

        // Calcular las fuerzas repulsivas para cada nodo.
        Parallel.ForEach(Graph.Nodes, node =>
        {
            root.ComputeRepulsiveForce(node);
        });
    }

    private QuadtreeNode BuildQuadtree()
    {
        var bounds = CalculateBounds(Graph.Nodes);

        var root = new QuadtreeNode(bounds, Configuration);

        foreach (var node in Graph.Nodes)
        {
            root.Insert(node);
        }

        root.ComputeCenterOfMass();

        return root;
    }

    private RectangleF CalculateBounds(IList<INode2D> nodes)
    {
        var minX = nodes.Min(n => n.X);
        var minY = nodes.Min(n => n.Y);
        var maxX = nodes.Max(n => n.X);
        var maxY = nodes.Max(n => n.Y);

        var margin = 10.0;

        return new RectangleF(
            (float)(minX - margin),
            (float)(minY - margin),
            (float)(maxX - minX + 2 * margin),
            (float)(maxY - minY + 2 * margin));
    }

    private void Attract()
    {
        // Atracción fuerza
        foreach (var edge in Graph.Edges)
        {
            var source = edge.Source;
            var target = edge.Target;

            if (source.IsFrozen && target.IsFrozen)
            {
                continue; // Si ambos nodos están congelados, no hay necesidad de calcular la fuerza.
            }

            var dx = target.X - source.X;
            var dy = target.Y - source.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy) + Configuration.Epsilon;

            var force = (distance - Configuration.EquilibriumDistance) * Configuration.AttractionForce * edge.Weight * (source.Weight + target.Weight) / 2.0;

            var forceX = force * dx / distance;
            var forceY = force * dy / distance;

            if (!source.IsFrozen)
            {
                source.ForceX += forceX;
                source.ForceY += forceY;
            }

            if (!target.IsFrozen)
            {
                target.ForceX -= forceX;
                target.ForceY -= forceY;
            }
        }
    }

    private void UpdatePositions()
    {
        foreach (var node in Graph.Nodes)
        {
            if (!node.IsFrozen)
            {
                node.X += node.ForceX / node.Weight * Configuration.Damping;
                node.Y += node.ForceY / node.Weight * Configuration.Damping;
            }

            // Reiniciar fuerzas para la siguiente iteración.
            node.ForceX = 0;
            node.ForceY = 0;
        }
    }

    public void Distribute(double width, double height)
    {
        var random = new Random();
        foreach (var node in Graph.Nodes)
        {
            node.X = random.NextDouble() * width;
            node.Y = random.NextDouble() * height;
        }
    }
}


public class QuadtreeNode
{
    private readonly Configuration _config;

    public RectangleF Bounds { get; }
    public List<INode2D> Nodes { get; }
    public QuadtreeNode[] Children { get; private set; } // NW, NE, SW, SE
    public Vector2D CenterOfMass { get; private set; }
    public double TotalMass { get; private set; }
    public bool IsLeaf => Children == null;

    // Constructor
    public QuadtreeNode(RectangleF bounds, Configuration config)
    {
        Bounds = bounds;
        Nodes = new List<INode2D>();
        _config = config;
    }

    /// <summary>
    /// Inserta un nodo en el Quadtree.
    /// </summary>
    public void Insert(INode2D node)
    {
        if (!Bounds.Contains((float)node.X, (float)node.Y))
        {
            // El nodo está fuera de los límites de este cuadrante.
            return;
        }

        if (IsLeaf && Nodes.Count < _config.MaxNodesPerLeaf)
        {
            // Añadir el nodo a esta hoja.
            Nodes.Add(node);
        }
        else
        {
            if (IsLeaf)
            {
                // Subdividir este nodo en cuatro hijos.
                Subdivide();

                // Mover nodos existentes a los hijos correspondientes.
                foreach (var existingNode in Nodes)
                {
                    InsertIntoChildren(existingNode);
                }
                Nodes.Clear();
            }

            // Insertar el nuevo nodo en el hijo adecuado.
            InsertIntoChildren(node);
        }
    }

    /// <summary>
    /// Calcula el centro de masa y la masa total para este nodo y sus hijos.
    /// </summary>
    public void ComputeCenterOfMass()
    {
        if (IsLeaf)
        {
            TotalMass = Nodes.Sum(n => n.Weight);
            if (TotalMass > 0)
            {
                var sumX = Nodes.Sum(n => n.X * n.Weight);
                var sumY = Nodes.Sum(n => n.Y * n.Weight);
                CenterOfMass = new Vector2D(sumX / TotalMass, sumY / TotalMass);
            }
            else
            {
                CenterOfMass = new Vector2D(0, 0);
            }
        }
        else
        {
            TotalMass = 0;
            var sumX = 0.0;
            var sumY = 0.0;

            foreach (var child in Children)
            {
                if (child != null)
                {
                    child.ComputeCenterOfMass();
                    TotalMass += child.TotalMass;
                    sumX += child.CenterOfMass.X * child.TotalMass;
                    sumY += child.CenterOfMass.Y * child.TotalMass;
                }
            }

            if (TotalMass > 0)
            {
                CenterOfMass = new Vector2D(sumX / TotalMass, sumY / TotalMass);
            }
            else
            {
                CenterOfMass = new Vector2D(0, 0);
            }
        }
    }

    /// <summary>
    /// Calcula la fuerza repulsiva sobre un nodo dado.
    /// </summary>
    public void ComputeRepulsiveForce(INode2D node)
    {
        if (IsLeaf && (Nodes.Count == 1 && Nodes[0] == node))
        {
            // No interactúa consigo mismo.
            return;
        }

        var dx = CenterOfMass.X - node.X;
        var dy = CenterOfMass.Y - node.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy) + _config.Epsilon;

        // Tamaño del cuadrante.
        var s = Bounds.Width;

        if ((s / distance) < _config.Theta || IsLeaf)
        {
            // Utilizar aproximación de Barnes-Hut.
            var force = _config.RepulsionForce * node.Weight * TotalMass / (distance * distance);
            var forceX = force * dx / distance;
            var forceY = force * dy / distance;

            if (!node.IsFrozen)
            {
                node.ForceX -= forceX;
                node.ForceY -= forceY;
            }
        }
        else
        {
            // Recorrer los hijos.
            foreach (var child in Children)
            {
                if (child != null)
                {
                    child.ComputeRepulsiveForce(node);
                }
            }
        }
    }

    /// <summary>
    /// Subdivide este nodo en cuatro hijos.
    /// </summary>
    private void Subdivide()
    {
        Children = new QuadtreeNode[4];

        var halfWidth = Bounds.Width / 2f;
        var halfHeight = Bounds.Height / 2f;
        var x = Bounds.X;
        var y = Bounds.Y;

        Children[0] = new QuadtreeNode(new RectangleF(x, y, halfWidth, halfHeight), _config); // NW
        Children[1] = new QuadtreeNode(new RectangleF(x + halfWidth, y, halfWidth, halfHeight), _config); // NE
        Children[2] = new QuadtreeNode(new RectangleF(x, y + halfHeight, halfWidth, halfHeight), _config); // SW
        Children[3] = new QuadtreeNode(new RectangleF(x + halfWidth, y + halfHeight, halfWidth, halfHeight), _config); // SE
    }

    /// <summary>
    /// Inserta un nodo en los hijos apropiados.
    /// </summary>
    private void InsertIntoChildren(INode2D node)
    {
        foreach (var child in Children)
        {
            if (child.Bounds.Contains((float)node.X, (float)node.Y))
            {
                child.Insert(node);
                return;
            }
        }
        // Si el nodo no encaja exactamente en un hijo, podrías manejar este caso según tus necesidades.
    }
}

