using System.Collections.ObjectModel;
using DynamicData;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;
using Zafiro.UI.Jobs.Manager;

namespace Zafiro.Avalonia.Controls.StatusBar;

public class StatusBar : IStatusBar
{
    public StatusBar(IActivatableJobManager jobManager, IStatus status)
    {
        jobManager.Jobs.Permanent().Transform(IJobViewModel (job) => new JobViewModel((IActivatableJob) job.Job)).Bind(out var permanentJobs).Subscribe();
        PermanentJobs = permanentJobs;
        
        jobManager.Jobs.Transient().Transform(IJobViewModel (job) => new JobViewModel((IActivatableJob) job.Job)).Bind(out var transientJobs).Subscribe();
        TransientJobs = transientJobs;
        
        Statuses = status.Statuses;
    }

    public ReadOnlyObservableCollection<IJobViewModel> PermanentJobs { get; }
    public ReadOnlyObservableCollection<IJobViewModel> TransientJobs { get; }
    public IObservable<object> Statuses { get; }
}