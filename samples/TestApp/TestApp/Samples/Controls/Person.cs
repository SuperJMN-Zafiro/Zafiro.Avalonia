using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Diagrams;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Controls;

public partial class Person : ReactiveObject, IHaveLocation, IMutableNode
{
    public string Name { get; }
    public double ForceX { get; set; }
    public double ForceY { get; set; }

    [Reactive] private double x;
    [Reactive] private double y;
    [Reactive] private double left;
    [Reactive] private double top;
    [Reactive] private bool isFrozen;

    /// <inheritdoc/>
    public Person(string name, double weight)
    {
        Name = name;
        Weight = weight;
        this.WhenAnyValue(person => person.Left).BindTo(this, person => person.X);
        this.WhenAnyValue(person => person.Top).BindTo(this, person => person.Y);
        this.WhenAnyValue(person => person.X).BindTo(this, person => person.left);
        this.WhenAnyValue(person => person.Y).BindTo(this, person => person.Top);
    }

    public double Weight { get; }
    
    public ICommand Unfreeze => ReactiveCommand.Create(() => IsFrozen = false);
    public ICommand Freeze => ReactiveCommand.Create(() => IsFrozen = true);
}