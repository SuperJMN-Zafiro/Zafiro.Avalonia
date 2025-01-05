using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Zafiro.Avalonia.Drawing;
using Zafiro.Avalonia.Drawing.LineStrategies;

namespace Zafiro.Avalonia.Controls.Diagrams.Simple;

public class DiagramView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<IHaveLocation>> NodesProperty =
        AvaloniaProperty.Register<DiagramView, IEnumerable<IHaveLocation>>(
            nameof(Nodes));

    public static readonly StyledProperty<IEnumerable<IHaveFromTo>> EdgesProperty =
        AvaloniaProperty.Register<DiagramView, IEnumerable<IHaveFromTo>>(
            nameof(Edges));

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
        AvaloniaProperty.Register<DiagramView, IDataTemplate>(
            nameof(ItemTemplate));

    public static readonly StyledProperty<ILineStrategy> ConnectionStyleProperty =
        AvaloniaProperty.Register<DiagramView, ILineStrategy>(
            nameof(ConnectionStyle), SLineStrategy.Instance);

    public static readonly StyledProperty<IBrush> ConnectionStrokeProperty =
        AvaloniaProperty.Register<DiagramView, IBrush>(nameof(ConnectionStroke), Brushes.Black);

    public static readonly StyledProperty<double> ConnectionStrokeThicknessProperty =
        AvaloniaProperty.Register<DiagramView, double>(
            nameof(ConnectionStrokeThickness), 1d);

    public IEnumerable<IHaveLocation> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public IEnumerable<IHaveFromTo> Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public ILineStrategy ConnectionStyle
    {
        get => GetValue(ConnectionStyleProperty);
        set => SetValue(ConnectionStyleProperty, value);
    }

    public IBrush ConnectionStroke
    {
        get => GetValue(ConnectionStrokeProperty);
        set => SetValue(ConnectionStrokeProperty, value);
    }

    public double ConnectionStrokeThickness
    {
        get => GetValue(ConnectionStrokeThicknessProperty);
        set => SetValue(ConnectionStrokeThicknessProperty, value);
    }
}