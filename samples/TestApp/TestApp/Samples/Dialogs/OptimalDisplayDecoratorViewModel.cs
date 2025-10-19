using ReactiveUI;
using Zafiro.Avalonia.Controls;
using Zafiro.UI;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Dialogs;

[Section(name: "Dialogs", icon: "fa-comment-dots", sortIndex: 5)]
public class OptimalDisplayDecoratorViewModel : ReactiveObject, IViewModel
{
    private bool isCentered = true;
    private double maxProportion = 0.7;
    private double minProportion = 0.3;

    public double MinProportion
    {
        get => minProportion;
        set => this.RaiseAndSetIfChanged(ref minProportion, value);
    }

    public double MaxProportion
    {
        get => maxProportion;
        set => this.RaiseAndSetIfChanged(ref maxProportion, value);
    }

    public bool IsCentered
    {
        get => isCentered;
        set
        {
            this.RaiseAndSetIfChanged(ref isCentered, value);
            this.RaisePropertyChanged(nameof(ContentAlignment));
        }
    }

    public ContentAlignment ContentAlignment => IsCentered ? ContentAlignment.Center : ContentAlignment.Start;
}