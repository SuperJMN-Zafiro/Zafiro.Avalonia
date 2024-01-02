using System;
using System.Reactive;
using ReactiveUI;
using Zafiro.Reactive;

namespace TestApp.ViewModels;

public class BehaviorsSampleViewModel : ViewModelBase
{
    public BehaviorsSampleViewModel()
    {
        Close = ReactiveCommand.Create(() => { });
        Executed = Close.ToSignal();
    }

    public IObservable<Unit> Executed { get; set; }

    public ReactiveCommand<Unit, Unit> Close { get; set; }
}
