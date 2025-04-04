using System.Collections.ObjectModel;
using Zafiro.Avalonia.Controls.StatusBar.Jobs;
using Zafiro.Avalonia.Controls.StatusBar.NotificationTypes;

namespace Zafiro.Avalonia.Controls.StatusBar.Design;

public class StatusBarDesign : IStatusBar
{
    public IObservable<object> Statuses { get; } = Observable.Return("Sample status");

    public ReadOnlyObservableCollection<IJobViewModel> PermanentJobs { get; } = new([
        new JobViewModelDesign
        {
            Name = "Test Job",
            Icon = new ImageContent(new Uri("avares://DNAGedcom/Assets/Images/ancestry-logo.png")),
        },
        new JobViewModelDesign
        {
            Name = "Test Job",
            Icon = new ImageContent(new Uri("avares://DNAGedcom/Assets/Images/gedmatch-logo.png")),
        }
    ]);

    public ReadOnlyObservableCollection<IJobViewModel> TransientJobs { get; }  = new([
        new JobViewModelDesign()
        {
            Name = "Transient Job",
            Status = Observable.Return(new MessageWithPathContent("Test", "iconTick")),
            // Icon = new LogoViewModel(new Uri("avares://DNAGedcom/Assets/Images/cma.png")),
        }
    ]);
}