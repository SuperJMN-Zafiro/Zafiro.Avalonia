using System.Collections.ObjectModel;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;

namespace Zafiro.Avalonia.Controls.StatusBar;

public interface IStatusBar
{
    IObservable<object> Statuses { get; }
    ReadOnlyObservableCollection<IJobViewModel> PermanentJobs { get; }
    ReadOnlyObservableCollection<IJobViewModel> TransientJobs { get; }
}