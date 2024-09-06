using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Zafiro.Actions;
using Zafiro.Progress;
using Zafiro.Reactive;
using Zafiro.UI;
using Zafiro.UI.Jobs;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.FileExplorer.Core.Transfers;

internal class TransferItem : ReactiveObject, ITransferItem 
{
    public TransferItem(string description, IAction<LongProgress> action)
    {
        Description = description;
        Action = action;
        Progress = action.Progress.Sample(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler).ReplayLastActive();
        Id = description;
        Name = description;
        Execution = ExecutionFactory.From(ct => action.Execute(ct), action.Progress.Select(x => new ProportionalProgress(x.Value)), Maybe.None);
        Status = Observable.Return(description);
        Icon = null;
    }

    public IObservable<LongProgress> Progress { get; }

    public string Description { get; }
    public IAction<LongProgress> Action { get; }
    public string Id { get; }
    public string Name { get; }
    public object Icon { get; }
    
    public IObservable<object> Status { get; }
    public IExecution Execution { get; }
}