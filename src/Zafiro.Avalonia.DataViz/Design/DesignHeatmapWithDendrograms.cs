using Zafiro.DataAnalysis;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace Zafiro.Avalonia.DataViz.Design;

public class DesignHeatmapWithDendrograms : IHeatmapWithDendrograms
{
    private readonly IHeatmapWithDendrograms inner;

    public DesignHeatmapWithDendrograms()
    {
        var strategy = new SingleLinkageClusteringStrategy<string>();
        inner = HeatmapWithDendrograms.Create(new DesignTable(), strategy, strategy);
    }

    public ITable Table => inner.Table;

    public ICluster RowsCluster => inner.RowsCluster;

    public ICluster ColumnsCluster => inner.ColumnsCluster;
}