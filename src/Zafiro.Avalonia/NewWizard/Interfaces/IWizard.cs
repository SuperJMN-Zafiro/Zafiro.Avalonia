using ReactiveUI;

namespace Zafiro.Avalonia.NewWizard.Interfaces;

public interface IWizard
{
    IObservable<IPage> CurrentPageWizard { get; }
    IList<IPage> PagesList { get; }
    IReactiveCommand GoNextCommand { get; }
    IReactiveCommand GoBackCommand { get; }
    public IObservable<bool> IsFinished { get; }
}

public interface IWizard<TResult, TPage>
{
    IObservable<TPage> CurrentPage { get; }
    IMyCommand GoNext { get; }
    IMyCommand GoBack { get; }
    Task<TResult> Result { get; }
}