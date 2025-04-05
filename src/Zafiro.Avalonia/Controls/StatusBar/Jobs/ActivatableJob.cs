using System.Windows.Input;
using Zafiro.UI.Jobs;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.Controls.StatusBar.Jobs;

public class ActivatableJob(IJob job, ICommand? activateCommand = null) : IActivatableJob
{
    public IJob Job { get; } = job;

    public string Id { get; } = job.Id;
    public string Name { get; } = job.Name;
    public object Icon { get; } = job.Icon;
    public IObservable<object> Status { get; } = job.Status;
    public IExecution Execution { get; } = job.Execution;
    public ICommand? ActivateCommand { get; } = activateCommand;
}