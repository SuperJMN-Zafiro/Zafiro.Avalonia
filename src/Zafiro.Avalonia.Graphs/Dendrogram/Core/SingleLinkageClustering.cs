using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Avalonia.DataViz.Dendrogram.Core;

public class SingleLinkageClustering : IClusteringAlgorithm
{
    public Cluster Clusterize(List<Tuple<object, object, double>> distances)
    {
        var clusters = new List<Cluster>();
        var elements = distances.SelectMany(d => new[] { d.Item1, d.Item2 }).Distinct().ToList();

        // Crear clústeres iniciales (hojas)
        foreach (var element in elements)
            clusters.Add(new Cluster(element));

        while (clusters.Count > 1)
        {
            // Obtener los dos clústeres más cercanos y la distancia de fusión
            var closestPair = FindClosestPair(clusters, distances);
            var c1 = closestPair.Item1;
            var c2 = closestPair.Item2;
            var distance = closestPair.Item3;

            Console.WriteLine($"Merging {c1} and {c2} at distance {distance}");

            clusters.Remove(c1);
            clusters.Remove(c2);

            // Crear nuevo clúster fusionado (se establecerá el padre automáticamente)
            var mergedCluster = new Cluster(c1, c2, distance);
            clusters.Add(mergedCluster);
        }

        // Retornar el clúster raíz del dendrograma
        return clusters[0];
    }

    private Tuple<Cluster, Cluster, double> FindClosestPair(List<Cluster> clusters, List<Tuple<object, object, double>> distances)
    {
        Cluster c1 = null, c2 = null;
        double minDistance = double.MaxValue;

        foreach (var cluster1 in clusters)
        {
            foreach (var cluster2 in clusters)
            {
                if (cluster1 == cluster2) continue;

                // Obtener la distancia entre los clústeres
                var dist = GetDistance(cluster1, cluster2, distances);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    c1 = cluster1;
                    c2 = cluster2;
                }
            }
        }

        // Retornamos los dos clústeres más cercanos y la distancia entre ellos
        return Tuple.Create(c1, c2, minDistance);
    }

    private double GetDistance(Cluster c1, Cluster c2, List<Tuple<object, object, double>> distances)
    {
        double minDistance = double.MaxValue;

        foreach (var e1 in c1.AllElements)
        {
            foreach (var e2 in c2.AllElements)
            {
                var distance = distances.FirstOrDefault(d => (d.Item1 == e1 && d.Item2 == e2) || (d.Item1 == e2 && d.Item2 == e1));
                if (distance != null)
                {
                    minDistance = Math.Min(minDistance, distance.Item3);
                }
            }
        }

        return minDistance;
    }
}
