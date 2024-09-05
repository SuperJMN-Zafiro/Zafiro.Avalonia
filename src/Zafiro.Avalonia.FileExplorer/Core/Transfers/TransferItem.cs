using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Zafiro.Actions;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core.Transfers;

internal class TransferItem : ReactiveObject, ITransferItem
{
    public TransferItem(string description, IAction<LongProgress> action)
    {
        Description = description;
        Action = action;
        Transfer = StoppableCommand.CreateFromTask(ct => Action.Execute(ct, ThreadPoolScheduler.Instance), Maybe<IObservable<bool>>.None);
        Progress = action.Progress.Sample(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler).ReplayLastActive();
    }

    public IObservable<LongProgress> Progress { get; }

    public IStoppableCommand<Unit, Result> Transfer { get; }

    public string Description { get; }

    public IAction<LongProgress> Action { get; }
}