using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Zafiro.Avalonia.Drawing;
using Zafiro.Avalonia.Drawing.RectConnectorStrategies;
using Zafiro.DataAnalysis.Graphs;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class LabelsControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<IEdge<INode>>> EdgesProperty =
        AvaloniaProperty.Register<LabelsControl, IEnumerable<IEdge<INode>>>(
            nameof(Edges));

    public static readonly DirectProperty<LabelsControl, IEnumerable> LabelsProperty =
        AvaloniaProperty.RegisterDirect<LabelsControl, IEnumerable>(
            nameof(Labels), o => o.Labels, (o, v) => o.Labels = v);

    public static readonly StyledProperty<IConnectorStrategy> ConnectorStyleProperty =
        AvaloniaProperty.Register<LabelsControl, IConnectorStrategy>(
            nameof(ConnectorStyle), SLineConnectorStrategy.Instance);


    public static readonly StyledProperty<IDataTemplate?> LabelTemplateProperty =
        AvaloniaProperty.Register<LabelsControl, IDataTemplate?>(
            nameof(LabelTemplate));

    public static readonly StyledProperty<ItemsControl> HostProperty = AvaloniaProperty.Register<LabelsControl, ItemsControl>(
        nameof(Host));

    public ItemsControl Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    private IEnumerable labels;

    public LabelsControl()
    {
        this.WhenAnyValue(x => x.Edges)
            .WhereNotNull()
            .Select(edges => edges.Select(edge =>
            {
                var positionUpdated = PositionUpdated(edge, edges);

                var content = LabelTemplate?.Build(null);

                if (content != null)
                {
                    content.DataContext = edge;
                }
                
                var canvasItem = new CanvasContent(positionUpdated)
                {
                    Content = content
                };

                return canvasItem;
            }))
            .Do(canvasContents => Labels = canvasContents)
            .Subscribe();
    }

    public IDataTemplate? LabelTemplate
    {
        get => GetValue(LabelTemplateProperty);
        set => SetValue(LabelTemplateProperty, value);
    }

    public IEnumerable<IEdge<INode>> Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public IEnumerable Labels
    {
        get => labels;
        private set => SetAndRaise(LabelsProperty, ref labels, value);
    }

    public IConnectorStrategy ConnectorStyle
    {
        get => GetValue(ConnectorStyleProperty);
        set => SetValue(ConnectorStyleProperty, value);
    }

    private IObservable<Point> PositionUpdated(IEdge<INode> edge, IEnumerable<IEdge<INode>> edges)
    {
        var manager = new ConnectionLayoutManager();
        
        return edge.BoundsChanged().Select(_ =>
        {
            if (Host != null)
            {
                var layout = manager.CalculateLayout(edges.ToList(), Host);

                var found = layout.Connections.First(x => x.Edge == edge);
                return ConnectorStyle.LabelPosition(found.FromPoint, found.FromSide, found.ToPoint, found.ToSide);
            }
            
            return ConnectorStyle.LabelPosition(edge.From.Location(), Side.Right, edge.To.Location(), Side.Bottom);
        });
    }
}