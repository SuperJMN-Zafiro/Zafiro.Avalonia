using System.Reactive;
using ReactiveUI;
using Zafiro.UI;

namespace Zafiro.Avalonia.Wizard;


public interface IWizard
{
    ReactiveCommand<Unit, Unit> Finish { get; }
    IObservable<IPage> CurrentPage { get; }
    IList<IPage> PagesList { get; }
    IReactiveCommand GoNext { get; }
    IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public string FinishText { get; }
}

public interface IWizard<TResult> : IWizard, IResult<TResult>
{
}