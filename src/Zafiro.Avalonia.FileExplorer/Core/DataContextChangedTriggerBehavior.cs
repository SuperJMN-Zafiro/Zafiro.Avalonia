using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.FileExplorer.Core;

public class DataContextChangedTriggerBehavior: Trigger<StyledElement>
{
    private readonly CompositeDisposable disposable = new();
    
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject
            .WhenAnyValue(x => x.DataContext)
            .Do(o => Interaction.ExecuteActions(this, Actions, null))
            .Subscribe()
            .DisposeWith(disposable);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        disposable.Dispose();
    }
}