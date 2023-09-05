using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.Avalonia.Model;

public interface IWizard
{
    IObservable<IWizardPage> ActivePage { get; }
    IList<IWizardPage> Pages { get; }
    ICommand GoNextCommand { get; set; }
    IReactiveCommand BackCommand { get; set; }
    IObservable<bool> CanGoNext { get; }
}