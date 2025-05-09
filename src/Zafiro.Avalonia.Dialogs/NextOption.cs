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
    private readonly ISlimWizard wizard;
    [Reactive] private IEnhancedCommand command;
    [Reactive] private string? title;

    public NextOption(ISlimWizard wizard)
    {
        this.wizard = wizard;
        this.WhenAnyValue(x => x.wizard.Next).BindTo(this, x => x.Command).DisposeWith(disposables);
        this.WhenAnyValue(x => x.wizard.Next.Text).BindTo(this, x => x.Title).DisposeWith(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    public bool IsDefault { get; } = true;
    public bool IsCancel { get; } = false;
    public IObservable<bool> IsVisible { get; } = Observable.Return(true);
    public OptionRole Role { get; } = OptionRole.Primary;
}