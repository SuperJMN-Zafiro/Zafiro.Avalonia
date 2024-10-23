using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class DiagramView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<IHaveLocation>> NodesProperty = AvaloniaProperty.Register<DiagramView, IEnumerable<IHaveLocation>>(
        nameof(Nodes));

    public IEnumerable<IHaveLocation> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<IHaveFromTo>> EdgesProperty = AvaloniaProperty.Register<DiagramView, IEnumerable<IHaveFromTo>>(
        nameof(Edges));

    public IEnumerable<IHaveFromTo> Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<DiagramView, IDataTemplate>(
        nameof(ItemTemplate));

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
}

public interface IHaveFromTo
{
    public object From { get; }
    public object To { get; }
}

public interface IHaveLocation
{
    public double Left { get; set; }
    public double Top { get; set; }
}