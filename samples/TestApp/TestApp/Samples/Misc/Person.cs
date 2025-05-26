using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Diagrams;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Misc;

public partial class Person : ReactiveObject, IHaveLocation, IMutableNode
{
    [Reactive] private bool isFrozen;
    [Reactive] private double left;
    [Reactive] private double top;

    [Reactive] private double x;
    [Reactive] private double y;

    /// <inheritdoc/>
    public Person(string name, double weight)
    {
        Name = name;
        Weight = weight;
        this.WhenAnyValue<Person, double>(person => person.Left).BindTo<double, Person, double>(this, person => person.X);
        this.WhenAnyValue<Person, double>(person => person.Top).BindTo<double, Person, double>(this, person => person.Y);
        this.WhenAnyValue<Person, double>(person => person.X).BindTo(this, person => person.left);
        this.WhenAnyValue<Person, double>(person => person.Y).BindTo<double, Person, double>(this, person => person.Top);
    }

    public string Name { get; }

    public ICommand Unfreeze => ReactiveCommand.Create<bool>(() => IsFrozen = false);
    public ICommand Freeze => ReactiveCommand.Create<bool>(() => IsFrozen = true);
    public double ForceX { get; set; }
    public double ForceY { get; set; }

    public double Weight { get; }
}