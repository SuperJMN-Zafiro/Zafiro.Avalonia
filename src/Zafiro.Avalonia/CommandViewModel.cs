using System.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia;

public class CommandViewModel : ReactiveObject
{
    public CommandViewModel()
    {
        NeverEndingCommand = ReactiveCommand.CreateFromObservable(Observable.Never<Unit>).Enhance();
        RegularCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Delay(2000);
            return 123;
        }).Enhance();
        NeverEndingCommand.Execute().Subscribe();
    }

    public IEnhancedCommand<Unit, int> RegularCommand { get; set; }

    public IEnhancedUnitCommand NeverEndingCommand { get; set; }
}