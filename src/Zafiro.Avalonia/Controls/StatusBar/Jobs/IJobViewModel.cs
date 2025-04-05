using System.Windows.Input;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.Controls.StatusBar.Jobs;

public interface IJobViewModel
{
    public string Name { get; }
    public IObservable<object> Status { get; }
    public IExecution Execution { get; }
    public object Icon { get; }
    public IObservable<bool> IsExecuting { get; }
    public ICommand? ActivateCommand { get; }
}