using System.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia;

public class CommandViewModel : ReactiveObject
{
    public CommandViewModel()
    {
        NeverEndingCommand = EnhancedCommand.Create(ReactiveCommand.CreateFromObservable(Observable.Never<Unit>));
        RegularCommand = EnhancedCommand.Create(ReactiveCommand.Create(() => 123));
        NeverEndingCommand.Execute().Subscribe();
    }

    public EnhancedCommand<Unit, int> RegularCommand { get; set; }

    public EnhancedCommand<Unit, Unit> NeverEndingCommand { get; set; }
}