using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zafiro.Avalonia.Graphs.Core;

public class Engine
{
    public Engine(IGraph2D graph)
    {
        Graph = graph;
    }

    public Configuration Configuration { get; } = new();

    public IGraph2D Graph { get; }

    public void Step()
    {
        ResetForces();
        Repel();
        Attract();
        UpdatePositions();
    }

    private void ResetForces()
    {
        foreach (var node in Graph.Nodes)
        {
            node.ForceX = 0;
            node.ForceY = 0;
        }
    }

    private void Repel()
    {
        var forceChanges = new Vector2D[Graph.Nodes.Count];

        Parallel.For(0, Graph.Nodes.Count, i =>
        {
            for (var j = i + 1; j < Graph.Nodes.Count; j++)
            {
                var nodeA = Graph.Nodes[i];
                var nodeB = Graph.Nodes[j];

                var dx = nodeB.X - nodeA.X;
                var dy = nodeB.Y - nodeA.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

                var force = Configuration.RepulsionForce * (nodeA.Weight * nodeB.Weight)  / (distance * distance);

                var forceX = force * dx / distance;
                var forceY = force * dy / distance;

                // Accumulate changes in forces in temporary variables
                forceChanges[i] -= new Vector2D(forceX, forceY);
                forceChanges[j] += new Vector2D(forceX, forceY);
            }
        });

        // Apply them when all calculations have finished
        for (int i = 0; i < Graph.Nodes.Count; i++)
        {
            Graph.Nodes[i].ForceX += forceChanges[i].X;
            Graph.Nodes[i].ForceY += forceChanges[i].Y;
        }
    }

    private void Attract()
    {
        // Attraction force
        foreach (var edge in Graph.Edges)
        {
            var linkTarget = edge.Target;
            var linkSource = edge.Source;

            var dx = linkTarget.X - linkSource.X;
            var dy = linkTarget.Y - linkSource.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy) + 0.01;

            var force = (distance - Configuration.EquilibriumDistance) * Configuration.AttractionForce * edge.Weight * (linkSource.Weight + linkTarget.Weight) / 2.0;

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
        foreach (var node in Graph.Nodes)
        {
            node.X += (node.ForceX / node.Weight) * Configuration.Damping;
            node.Y += (node.ForceY / node.Weight) * Configuration.Damping;
        }
    }

    public void Distribute(double width, double height)
    {
        Graph.Nodes.ToList().ForEach(x =>
        {
            x.X = Random.Shared.Next((int) width);
            x.Y = Random.Shared.Next((int) height);
        });
    }
}