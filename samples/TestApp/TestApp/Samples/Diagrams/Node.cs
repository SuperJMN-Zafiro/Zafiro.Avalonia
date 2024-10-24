using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Diagrams;

namespace TestApp.Samples.Diagrams;

public partial class Node(string name) : ReactiveObject, IHaveLocation
{
    public string Name { get; } = name;
    [Reactive] public double left;
    [Reactive] public double top;
}