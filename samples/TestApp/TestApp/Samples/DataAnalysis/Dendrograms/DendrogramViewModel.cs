using Zafiro.DataAnalysis.Clustering;

namespace TestApp.Samples.DataAnalysis.Dendrograms;

public class DendrogramViewModel
{
    public DendrogramViewModel()
    {
        Cluster<string> cluster = new Internal<string>(
            new Internal<string>(
                new Internal<string>(
                    new Leaf<string>("A"), 
                    new Leaf<string>("B"), 1),
                new Leaf<string>("F"), 2),
            new Internal<string>(
                new Leaf<string>("C"),
                new Internal<string>(
                    new Leaf<string>("D"),
                    new Leaf<string>("E"), 4), 5), 7);
        Cluster = ClusterNode.Create(cluster);
    }

    public ClusterNode Cluster { get; set; }
}