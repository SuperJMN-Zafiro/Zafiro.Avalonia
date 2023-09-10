using System.Windows.Input;

namespace Zafiro.Avalonia.NewWizard.Interfaces;

public interface IWizard
{
    IObservable<object> CurrentPageWizard { get; }
    IList<object> PagesList { get; }
    ICommand GoNextCommand { get; }
    ICommand GoBackCommand { get; }
    IObservable<bool> CanGoNext { get; }
    IObservable<bool> CanGoBack { get; }
    Task<object> Result { get; }
}

public interface IWizard<TResult, TPage>
{
    IObservable<TPage> CurrentPage { get; }
    IMyCommand GoNext { get; }
    IMyCommand GoBack { get; }
    Task<TResult> Result { get; }
}