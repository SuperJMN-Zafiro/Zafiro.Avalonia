using ReactiveUI;
using Zafiro.Avalonia.MigrateToZafiro;

namespace Zafiro.Avalonia.Wizard.Interfaces;

public interface IWizard
{
    IObservable<IPage> CurrentPageWizard { get; }
    IList<IPage> PagesList { get; }
    IReactiveCommand GoNextCommand { get; }
    IReactiveCommand GoBackCommand { get; }
    public IObservable<bool> IsFinished { get; }
}

public interface IWizard<TResult, TPage> : IHaveResult<TResult>
{
    IObservable<TPage> CurrentPage { get; }
    IMyCommand GoNext { get; }
    IMyCommand GoBack { get; }
    Task<TResult> Result { get; }
}