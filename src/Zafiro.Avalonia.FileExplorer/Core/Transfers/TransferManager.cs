using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using MoreLinq;
using Zafiro.Reactive;
using Zafiro.UI.Jobs;
using Zafiro.UI.Jobs.Manager;

namespace Zafiro.Avalonia.FileExplorer.Core.Transfers;

public class TransferManager : ITransferManager, IDisposable
{
    private readonly SourceCache<ITransferItem, object> items = new(item => item);
    private readonly CompositeDisposable disposable = new();
    private readonly JobManager jobManager;

    public TransferManager()
    {
        var itemChanges = items.Connect();

        jobManager = new JobManager();

        jobManager.Tasks
            .Transform(x => x.Job)
            .Bind(out var jobItems)
            .Subscribe()
            .DisposeWith(disposable);

        Jobs = jobItems;

        itemChanges
            .Bind(out var transfers)
            .Subscribe()
            .DisposeWith(disposable);

        Transfers = transfers;
        
        IsTransferring = jobManager.Tasks
            .FilterOnObservable(x => x.Job.Execution.Start.IsExecuting)
            .Count()
            .Select(i => i > 0);
    }

    public ReadOnlyObservableCollection<IJob> Jobs { get; }

    public IObservable<bool> IsTransferring { get; }

    public IObservable<double> Progress { get; }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }

    public void Add(params ITransferItem[] transfers)
    {
        transfers.ForEach(x => jobManager.Add(x, new JobOptions { AutoStart = true, }));
    }

    public void Dispose()
    {
        items.Dispose();
        disposable.Dispose();
    }
}