using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

public class ForceDirectedEngine(IGraph2D graph)
{
    public List<IEdge2D> Edges { get; } = graph.Edges.ToList();

    public List<INode2D> Nodes { get; } = graph.Nodes.ToList();

    public Configuration Configuration { get; } = new();

    public IGraph2D Graph { get; } = graph;

    public void Step()
    {
        ResetForces();
        Repel();
        Attract();
        UpdatePositions();
    }

    private void ResetForces()
    {
        foreach (var node in Nodes)
        {
            node.ForceX = 0;
            node.ForceY = 0;
        }
    }

    private void Repel()
    {
        var forceChanges = new Vector2D[Nodes.Count];

        Parallel.For(0, Nodes.Count, i =>
        {
            for (var j = i + 1; j < Nodes.Count; j++)
            {
                var nodeA = Nodes[i];
                var nodeB = Nodes[j];

                var dx = nodeB.X - nodeA.X;
                var dy = nodeB.Y - nodeA.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

                var force = Configuration.RepulsionForce * (nodeA.Weight * nodeB.Weight) / (distance * distance);

                var forceX = force * dx / distance;
                var forceY = force * dy / distance;

                // Accumulate changes in forces in temporary variables
                forceChanges[i] -= new Vector2D(forceX, forceY);
                forceChanges[j] += new Vector2D(forceX, forceY);
            }
        });

        // Apply them when all calculations have finished
        for (var i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].ForceX += forceChanges[i].X;
            Nodes[i].ForceY += forceChanges[i].Y;
        }
    }

    private void Attract()
    {
        // Attraction force
        foreach (var edge in Edges)
        {
            var from = edge.From;
            var to = edge.To;

            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

            var force = (distance - Configuration.EquilibriumDistance) * Configuration.AttractionForce * edge.Weight *
                (from.Weight + to.Weight) / 2.0;

            var forceX = force * dx / distance;
            var forceY = force * dy / distance;

            from.ForceX += forceX;
            from.ForceY += forceY;
            to.ForceX -= forceX;
            to.ForceY -= forceY;
        }
    }

    private void UpdatePositions()
    {
        foreach (var node in Nodes.Where(x => !x.IsFrozen))
        {
            node.X += node.ForceX / node.Weight * Configuration.Damping;
            node.Y += node.ForceY / node.Weight * Configuration.Damping;
        }
    }

    public void Distribute(double width, double height)
    {
        Nodes.ForEach(x =>
        {
            x.X = Random.Shared.Next((int) width);
            x.Y = Random.Shared.Next((int) height);
        });
    }
}