using System;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Diagrams;
using Zafiro.Avalonia.DataViz.Graph.Core;

namespace TestApp.Samples.Controls;

public partial class Person : ReactiveObject, IHaveLocation, INode2D
{
    public string Name { get; }
    public double ForceX { get; set; }
    public double ForceY { get; set; }

    [Reactive] private double x;
    [Reactive] private double y;
    [Reactive] private double left;
    [Reactive] private double top;
    [Reactive] private bool isEnabled = true;

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
    
    public ICommand Enable => ReactiveCommand.Create(() => IsEnabled = true);

    public ICommand Disable => ReactiveCommand.Create(() => IsEnabled = false);
}