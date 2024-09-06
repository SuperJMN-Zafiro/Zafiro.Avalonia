using System.Collections.ObjectModel;
using Zafiro.UI.Jobs;

namespace Zafiro.Avalonia.FileExplorer.Core.Transfers;

public interface ITransferManager
{
    public void Add(params ITransferItem[] transfers);
    IObservable<double> Progress { get; }
    IObservable<bool> IsTransferring { get; }
    ReadOnlyObservableCollection<IJob> Jobs { get; }
}