using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Control;

public class GraphControl : TemplatedControl
{
    public static readonly StyledProperty<IGraph> GraphProperty =
        AvaloniaProperty.Register<GraphControl, IGraph>(nameof(Graph));

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<GraphControl, IDataTemplate?>(
            nameof(ItemTemplate));

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

    public static readonly StyledProperty<IDataTemplate?> EdgeTemplateProperty = AvaloniaProperty.Register<GraphControl, IDataTemplate?>(
        nameof(EdgeTemplate));

    public IDataTemplate? EdgeTemplate
    {
        get => GetValue(EdgeTemplateProperty);
        set => SetValue(EdgeTemplateProperty, value);
    }
}