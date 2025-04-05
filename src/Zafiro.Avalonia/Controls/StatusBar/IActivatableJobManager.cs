using DynamicData;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;
using Zafiro.UI.Jobs.Manager;

namespace Zafiro.Avalonia.Controls.StatusBar;

public interface IActivatableJobManager
{
    public void Add(IActivatableJob job, JobOptions options);
    public IObservable<IChangeSet<JobItem, string>> Jobs { get; }
}