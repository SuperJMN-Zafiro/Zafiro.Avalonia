using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zafiro.Avalonia.Graphs;

public class Engine<TNode2D, TEdge> where TNode2D : class, INode2D where TEdge : IEdge<TNode2D>
{
    public Engine(Graph2D<TNode2D, TEdge> graph)
    {
        Graph = graph;
    }

    public Configuration Configuration { get; } = new();

    public Graph2D<TNode2D, TEdge> Graph { get; }

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

                var force = Configuration.RepulsionForce / (distance * distance);

                var forceX = force * dx / distance;
                var forceY = force * dy / distance;

                // Acummulate changes in forces in temporary variables
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
        foreach (var node in Graph.Nodes)
        {
            node.X += node.ForceX * Configuration.Damping;
            node.Y += node.ForceY * Configuration.Damping;
        }
    }

    public void Distribute(int width, int height)
    {
        Graph.Nodes.ToList().ForEach(x =>
        {
            x.X = Random.Shared.Next(width);
            x.Y = Random.Shared.Next(height);
        });
    }
}