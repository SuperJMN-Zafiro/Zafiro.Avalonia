using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphs;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Impl;

public class Engine<T>
{
    private readonly List<INode2D<T>> _nodes;

    public Engine(IGraph2D<T> graph)
    {
        Graph = graph;
        _nodes = graph.Nodes.ToList();
    }

    public Configuration Configuration { get; } = new();

    public IGraph2D<T> Graph { get; }

    public void Step()
    {
        ResetForces();
        Repel();
        Attract();
        UpdatePositions();
    }

    private void ResetForces()
    {
        foreach (var node in _nodes)
        {
            node.ForceX = 0;
            node.ForceY = 0;
        }
    }

    private void Repel()
    {
        var forceChanges = new Vector2D[_nodes.Count];

        Parallel.For(0, _nodes.Count, i =>
        {
            for (var j = i + 1; j < _nodes.Count; j++)
            {
                var nodeA = _nodes[i];
                var nodeB = _nodes[j];

                var dx = nodeB.X - nodeA.X;
                var dy = nodeB.Y - nodeA.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

                var force = Configuration.RepulsionForce / (distance * distance);

                var forceX = force * dx / distance;
                var forceY = force * dy / distance;

                // Acummulate changes in forces in temporary variables
                forceChanges[i] -= new Vector2D(forceX, forceY);
                forceChanges[j] += new Vector2D(forceX, forceY);
            }
        });

        // Apply them when all calculations have finished
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].ForceX += forceChanges[i].X;
            _nodes[i].ForceY += forceChanges[i].Y;
        }
    }

    private void Attract()
    {
        // Attraction force
        foreach (var edge in Graph.Edges)
        {
            var linkTarget = Graph.GetNode(edge.Target);
            var linkSource = Graph.GetNode(edge.Source);

            var dx = linkTarget.X - linkSource.X;
            var dy = linkTarget.Y - linkSource.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

            var force = (distance - Configuration.EquilibriumDistance) * Configuration.AttractionForce * edge.Weight;

            var forceX = force * dx / distance;
            var forceY = force * dy / distance;

            linkSource.ForceX += forceX;
            linkSource.ForceY += forceY;
            linkTarget.ForceX -= forceX;
            linkTarget.ForceY -= forceY;
        }
    }

    private void UpdatePositions()
    {
        // Update positions
        foreach (var node in _nodes)
        {
            node.X += node.ForceX * Configuration.Damping;
            node.Y += node.ForceY * Configuration.Damping;
        }
    }

    public void Distribute(int width, int height)
    {
        _nodes.ToList().ForEach(x =>
        {
            x.X = Random.Shared.Next(width);
            x.Y = Random.Shared.Next(height);
        });
    }
}

public struct Vector2D
{
    public double X { get; set; }
    public double Y { get; set; }

    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2D operator -(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2D operator *(Vector2D v, double scalar)
    {
        return new Vector2D(v.X * scalar, v.Y * scalar);
    }

    public static Vector2D operator /(Vector2D v, double scalar)
    {
        return new Vector2D(v.X / scalar, v.Y / scalar);
    }

    public double Magnitude()
    {
        return Math.Sqrt(X * X + Y * Y);
    }

    public Vector2D Normalize()
    {
        var magnitude = Magnitude();
        return new Vector2D(X / magnitude, Y / magnitude);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
