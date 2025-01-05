using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams.Enhanced;

public class EnhancedDiagramView : TemplatedControl
{
    public static readonly StyledProperty<IDataTemplate> NodeTemplateProperty = AvaloniaProperty.Register<EnhancedDiagramView, IDataTemplate>(
        nameof(NodeTemplate));

    [InheritDataTypeFromItems(nameof(Nodes), AncestorType = typeof(EnhancedDiagramView))]
    public IDataTemplate NodeTemplate
    {
        get => GetValue(NodeTemplateProperty);
        set => SetValue(NodeTemplateProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<INode>> NodesProperty = AvaloniaProperty.Register<EnhancedDiagramView, IEnumerable<INode>>(
        nameof(Nodes));

    public IEnumerable<INode> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<IEdge<INode>>> EdgesProperty = AvaloniaProperty.Register<EnhancedDiagramView, IEnumerable<IEdge<INode>>>(
        nameof(Edges));

    public IEnumerable<IEdge<INode>> Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public static readonly StyledProperty<IBrush> ConnectorBrushProperty = AvaloniaProperty.Register<EnhancedDiagramView, IBrush>(
        nameof(ConnectorBrush));

    public IBrush ConnectorBrush
    {
        get => GetValue(ConnectorBrushProperty);
        set => SetValue(ConnectorBrushProperty, value);
    }

    public static readonly StyledProperty<double> ConnectorThicknessProperty = AvaloniaProperty.Register<EnhancedDiagramView, double>(
        nameof(ConnectorThickness));

    public double ConnectorThickness
    {
        get => GetValue(ConnectorThicknessProperty);
        set => SetValue(ConnectorThicknessProperty, value);
    }
}