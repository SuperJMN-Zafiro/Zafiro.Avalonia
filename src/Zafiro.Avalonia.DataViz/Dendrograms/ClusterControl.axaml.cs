using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Zafiro.DataAnalysis.Clustering;

namespace Zafiro.Avalonia.DataViz.Dendrograms;

public class ClusterControl : TemplatedControl
{
    public static readonly StyledProperty<ICluster> ClusterProperty = AvaloniaProperty.Register<ClusterControl, ICluster>(
        nameof(Cluster));

    public ICluster Cluster
    {
        get => GetValue(ClusterProperty);
        set => SetValue(ClusterProperty, value);
    }

    public static readonly StyledProperty<IBrush> ConnectionBrushProperty = AvaloniaProperty.Register<ClusterControl, IBrush>(
        nameof(ConnectionBrush), Brushes.Black);

    public IBrush ConnectionBrush
    {
        get => GetValue(ConnectionBrushProperty);
        set => SetValue(ConnectionBrushProperty, value);
    }

    public static readonly StyledProperty<double> ConnectionThicknessProperty = AvaloniaProperty.Register<ClusterControl, double>(
        nameof(ConnectionThickness), 1);

    public double ConnectionThickness
    {
        get => GetValue(ConnectionThicknessProperty);
        set => SetValue(ConnectionThicknessProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<ClusterControl, IDataTemplate>(
        nameof(ItemTemplate));

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly StyledProperty<double> DistanceMultiplierProperty = AvaloniaProperty.Register<ClusterControl, double>(
        nameof(DistanceMultiplier), 100);

    public double DistanceMultiplier
    {
        get => GetValue(DistanceMultiplierProperty);
        set => SetValue(DistanceMultiplierProperty, value);
    }
}