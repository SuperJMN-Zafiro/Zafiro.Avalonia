using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.UI;

namespace Zafiro.Avalonia.Controls;


public interface ISuperWizard
{
    ReactiveCommand<Unit, Unit> Complete { get; }
    IObservable<IPage> CurrentPage { get; }
    IList<IPage> PagesList { get; }
    IReactiveCommand GoNext { get; }
    IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
}

public interface ISuperWizard<TResult> : ISuperWizard, IResult<TResult>
{
}