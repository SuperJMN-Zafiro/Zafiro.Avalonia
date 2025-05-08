using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace Zafiro.Avalonia.Dialogs;

public partial class NextOption : ReactiveObject, IOption, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly IWizard wizard;
    [Reactive] private string? title;

    public NextOption(IWizard wizard)
    {
        this.wizard = wizard;
        this.WhenAnyValue(x => x.wizard.NextText).BindTo(this, x => x.Title).DisposeWith(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    public IEnhancedCommand Command => wizard.NextCommand;
    public bool IsDefault { get; } = true;
    public bool IsCancel { get; } = false;
    public IObservable<bool> IsVisible { get; } = Observable.Return(true);
    public OptionRole Role { get; } = OptionRole.Primary;
}