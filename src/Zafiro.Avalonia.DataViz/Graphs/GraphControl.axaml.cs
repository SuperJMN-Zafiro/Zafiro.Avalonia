using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.DataViz.Graphs.Control;

public class GraphControl : TemplatedControl
{
    public static readonly StyledProperty<IGraph> GraphProperty =
        AvaloniaProperty.Register<GraphControl, IGraph>(nameof(Graph));

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<GraphControl, IDataTemplate?>(
            nameof(ItemTemplate));

    public static readonly StyledProperty<IDataTemplate?> EdgeTemplateProperty =
        AvaloniaProperty.Register<GraphControl, IDataTemplate?>(
            nameof(EdgeTemplate));

    public static readonly StyledProperty<MouseButton> DragButtonProperty =
        AvaloniaProperty.Register<GraphControl, MouseButton>(nameof(DragButton), MouseButton.Left);

    public IGraph Graph
    {
        get => GetValue(GraphProperty);
        set => SetValue(GraphProperty, value);
    }

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public IDataTemplate? EdgeTemplate
    {
        get => GetValue(EdgeTemplateProperty);
        set => SetValue(EdgeTemplateProperty, value);
    }

    public MouseButton DragButton
    {
        get => GetValue(DragButtonProperty);
        set => SetValue(DragButtonProperty, value);
    }
}