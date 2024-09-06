using Zafiro.Actions;
using Zafiro.UI;
using Zafiro.UI.Jobs;

namespace Zafiro.Avalonia.FileExplorer.Core.Transfers;

public interface ITransferItem : IJob
{
    public string Description { get; }

    IObservable<LongProgress> Progress { get; }
}