using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class LabelsControl : TemplatedControl
{
    public static readonly StyledProperty<IDataTemplate?> LabelTemplateProperty =
        AvaloniaProperty.Register<LabelsControl, IDataTemplate?>(
            nameof(LabelTemplate));


    public IDataTemplate? LabelTemplate
    {
        get => GetValue(LabelTemplateProperty);
        set => SetValue(LabelTemplateProperty, value);
    }
    
    private LabelsController? controller;

    public static readonly DirectProperty<LabelsControl, LabelsController?> ControllerProperty = AvaloniaProperty.RegisterDirect<LabelsControl, LabelsController?>(
        nameof(Controller), o => o.Controller, (o, v) => o.Controller = v);

    public LabelsController? Controller
    {
        get => controller;
        set => SetAndRaise(ControllerProperty, ref controller, value);
    }

    public static readonly StyledProperty<IEnumerable<IEdge<INode>>> EdgesProperty = AvaloniaProperty.Register<LabelsControl, IEnumerable<IEdge<INode>>>(
        nameof(Edges));

    public IEnumerable<IEdge<INode>> Edges
    {
        get => GetValue(EdgesProperty);
        set => SetValue(EdgesProperty, value);
    }

    public LabelsControl()
    {
        this.WhenAnyValue(x => x.Edges)
            .WhereNotNull()
            .Select(edges => new LabelsController(edges))
            .BindTo(this, x => x.Controller);
    }
}