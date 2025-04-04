using System.Windows.Input;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.Controls.StatusBar.Jobs;

public class JobViewModel(IActivatableJob job) : IJobViewModel
{
    public string Name => job.Name;
    public IObservable<object> Status => job.Status;
    public IExecution Execution => job.Execution;
    public object Icon => job.Icon;
    public IObservable<bool> IsExecuting => job.Execution.Start.IsExecuting;
    public ICommand? ActivateCommand { get; } = job.ActivateCommand;
}