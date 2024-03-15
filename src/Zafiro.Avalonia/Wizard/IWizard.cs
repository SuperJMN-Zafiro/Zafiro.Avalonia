using System.Reactive;

namespace Zafiro.Avalonia.Wizard;

public interface IWizard
{
    ReactiveCommandBase<Unit, Unit> Finish { get; }
    IObservable<IPage> CurrentPage { get; }
    IList<IPage> PagesList { get; }
    ReactiveCommand<Unit, LinkedListNode<IPage>?> GoNext { get; }
    ReactiveCommand<Unit, LinkedListNode<IPage>?> GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public string FinishText { get; }
}

public interface IWizard<TResult> : IWizard, IResult<TResult>
{
}