using System;
using System.Collections.Generic;

namespace Zafiro.Avalonia.DataViz.Dendrogram.Core;

public interface IClusteringAlgorithm
{
    Cluster Clusterize(List<Tuple<object, object, double>> distances);
}