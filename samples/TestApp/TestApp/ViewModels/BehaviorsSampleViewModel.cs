using System;
using System.Reactive;
using ReactiveUI;
using Zafiro.Mixins;

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
