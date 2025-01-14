using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Zafiro.Avalonia.Drawing;
using Zafiro.Avalonia.Drawing.RectConnectorStrategies;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class LabelsControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<IEdge<INode>>> EdgesProperty =
        AvaloniaProperty.Register<LabelsControl, IEnumerable<IEdge<INode>>>(
            nameof(Edges));

    public static readonly DirectProperty<LabelsControl, ReadOnlyObservableCollection<CanvasContent>> LabelsProperty =
        AvaloniaProperty.RegisterDirect<LabelsControl, ReadOnlyObservableCollection<CanvasContent>>(
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

    private ReadOnlyObservableCollection<CanvasContent> labels;
    private readonly CompositeDisposable disposables = new();

    public LabelsControl()
    {
        this.WhenAnyValue(x => x.Edges, x => x.LabelTemplate, x => x.Host, x => x.ConnectorStyle)
            .Where(a =>
            {
                var any = new object[] { a.Item1, a.Item2, a.Item3, a.Item4 }.Any(o => o is null);
                return !any;
            })
            .Select(tuple => new ConnectionController(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4))
            .BindTo(this, x => x.Controller)
            .DisposeWith(disposables);
    }

    private ConnectionController controller;

    public static readonly DirectProperty<LabelsControl, ConnectionController> ControllerProperty = AvaloniaProperty.RegisterDirect<LabelsControl, ConnectionController>(
        nameof(Controller), o => o.Controller, (o, v) => o.Controller = v);

    public ConnectionController Controller
    {
        get => controller;
        set => SetAndRaise(ControllerProperty, ref controller, value);
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

    public ReadOnlyObservableCollection<CanvasContent> Labels
    {
        get => labels;
        private set => SetAndRaise(LabelsProperty, ref labels, value);
    }

    public IConnectorStrategy ConnectorStyle
    {
        get => GetValue(ConnectorStyleProperty);
        set => SetValue(ConnectorStyleProperty, value);
    }
}