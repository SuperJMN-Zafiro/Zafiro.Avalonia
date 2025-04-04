using System.Windows.Input;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.Controls.StatusBar.Design;

public class JobViewModelDesign : IJobViewModel
{
    public string Name { get; set; }
    public IObservable<object> Status { get; set; } = Observable.Return("My status");
    public IExecution Execution { get; }
    public object Icon { get; set; }
    public IObservable<bool> IsExecuting { get; } = Observable.Return(true);
    public ICommand ActivateCommand { get; }
}