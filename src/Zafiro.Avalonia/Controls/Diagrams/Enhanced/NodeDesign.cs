namespace Zafiro.Avalonia.Controls.Diagrams.Enhanced;

public partial class NodeDesign : ReactiveObject, INode
{
    [ReactiveUI.SourceGenerators.Reactive]
    private double left;

    [ReactiveUI.SourceGenerators.Reactive]
    private double top;

    public string Name { get; set; }
}