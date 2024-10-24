using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Diagrams;
using Zafiro.Avalonia.DataViz.Graph.Core;

namespace TestApp.Samples.Controls;

public partial class Person(string name, double weight) : ReactiveObject, IHaveLocation, INode2D
{
    private double x;
    private double y;
    public string Name { get; } = name;
    public double ForceX { get; set; }
    public double ForceY { get; set; }

    public double X
    {
        get => x;
        set => this.RaiseAndSetIfChanged(ref x, value);
    }

    public double Y
    {
        get => y;
        set => this.RaiseAndSetIfChanged(ref y, value);
    }

    public double Weight { get; } = weight;

    [Reactive] private bool isEnabled = true;

    public double Left
    {
        get => X;
        set
        {
            X = value;
            this.RaisePropertyChanged();
        }
    }

    public double Top
    {
        get => Y;
        set
        {
            Y = value;
            this.RaisePropertyChanged();
        }
    }

    public ICommand Enable => ReactiveCommand.Create(() =>
    {
        return IsEnabled = true;
    });

    public ICommand Disable => ReactiveCommand.Create(() => IsEnabled = false);
}