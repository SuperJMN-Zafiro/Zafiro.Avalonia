using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.UI;

namespace Zafiro.Avalonia.Controls;


public interface ISuperWizard
{
    ReactiveCommand<Unit, Unit> Finish { get; }
    IObservable<ISuperPage> CurrentPage { get; }
    IList<ISuperPage> PagesList { get; }
    IReactiveCommand GoNext { get; }
    IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public string FinishText { get; }
}

public interface ISuperWizard<TResult> : ISuperWizard, IResult<TResult>
{
}