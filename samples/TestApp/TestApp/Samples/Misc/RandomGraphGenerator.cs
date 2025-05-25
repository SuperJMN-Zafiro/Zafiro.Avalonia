using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp.Samples.Controls;

public class RandomGraphGenerator
{
    private Random random = new Random();

    public (List<Person> nodes, List<Friendship> edges) GenerateRandomGraph(int maxNodes, int maxEdgesPerNode)
    {
        var nodes = GenerateNodes(maxNodes);
        var edges = GenerateEdges(nodes, maxEdgesPerNode);
        return (nodes, edges);
    }

    private List<Person> GenerateNodes(int maxNodes)
    {
        var nodes = new List<Person>();
        for (int i = 0; i < maxNodes; i++)
        {
            nodes.Add(new Person($"Person_{i}", Random.Shared.NextDouble() * 50));
        }

        return nodes;
    }

    private List<Friendship> GenerateEdges(List<Person> nodes, int maxEdgesPerNode)
    {
        var edges = new List<Friendship>();
        foreach (var node in nodes)
        {
            var edgeCount = random.Next(0, maxEdgesPerNode + 1);
            var potentialFriends = nodes.Where(n => n != node && !edges.Any(e => (e.From == node && e.To == n) || (e.From == n && e.To == node))).ToList();

            for (int i = 0; i < edgeCount && potentialFriends.Any(); i++)
            {
                var friend = potentialFriends[random.Next(potentialFriends.Count)];
                var strength = random.NextDouble();
                edges.Add(new Friendship(node, friend, strength));
                potentialFriends.Remove(friend);
            }
        }

        return edges;
    }
}