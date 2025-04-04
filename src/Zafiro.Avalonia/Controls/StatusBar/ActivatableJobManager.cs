using DynamicData;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;
using Zafiro.UI.Jobs.Manager;

namespace Zafiro.Avalonia.Controls.StatusBar;

public class ActivatableJobManager : IActivatableJobManager
{
    public IJobManager JobManager { get; }

    public ActivatableJobManager(IJobManager jobManager)
    {
        JobManager = jobManager;
    }

    public void Add(IActivatableJob job, JobOptions options)
    {
        JobManager.Add(job, options);
    }

    public IObservable<IChangeSet<JobItem, string>> Jobs => JobManager.Tasks;
}