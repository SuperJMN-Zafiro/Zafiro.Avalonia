using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.Avalonia.Model;

public interface IWizard
{
    IPage CurrentPage { get; }
    IEnumerable<IPage> Pages { get; }
    ICommand GoNext { get; }
    IReactiveCommand GoBack { get; }
    IObservable<bool> CanGoNext { get; }
}