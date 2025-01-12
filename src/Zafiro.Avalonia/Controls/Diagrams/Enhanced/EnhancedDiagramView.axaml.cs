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

    public static readonly StyledProperty<IDataTemplate> LabelTemplateProperty = AvaloniaProperty.Register<EnhancedDiagramView, IDataTemplate>(
        nameof(LabelTemplate));

    [InheritDataTypeFromItems(nameof(Edges), AncestorType = typeof(EnhancedDiagramView))]
    public IDataTemplate LabelTemplate
    {
        get => GetValue(LabelTemplateProperty);
        set => SetValue(LabelTemplateProperty, value);
    }

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
        nameof(ConnectorBrush), defaultValue: Brushes.Black);

    public IBrush ConnectorBrush
    {
        get => GetValue(ConnectorBrushProperty);
        set => SetValue(ConnectorBrushProperty, value);
    }

    public static readonly StyledProperty<double> ConnectorThicknessProperty = AvaloniaProperty.Register<EnhancedDiagramView, double>(
        nameof(ConnectorThickness), defaultValue: 1.0D);

    public double ConnectorThickness
    {
        get => GetValue(ConnectorThicknessProperty);
        set => SetValue(ConnectorThicknessProperty, value);
    }
}