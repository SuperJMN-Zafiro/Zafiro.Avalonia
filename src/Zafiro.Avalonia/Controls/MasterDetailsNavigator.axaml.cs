using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Primitives;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls;

public class MasterDetailsNavigator : TemplatedControl
{
    public MasterDetailsNavigator()
    {
        SourceCache<MasterDetailsView, int> source = new(navigator => navigator.GetHashCode());

        MessageBus.Current.Listen<RegisterNavigation>()
            .Do(navigation => source.AddOrUpdate(navigation.MasterDetailsView))
            .Subscribe();

        var backCommands = source
            .Connect()
            .AutoRefresh(x => x.AreDetailsShown)
            .AutoRefresh(x => x.IsCollapsed)
            .Filter(masterDetailsView => masterDetailsView is { AreDetailsShown: true, IsCollapsed: true })
            .Transform(masterDetailsView => ReactiveCommand.Create(masterDetailsView.HideDetails));

        CanNavigateBack = backCommands.Count().Select(i => i > 0);
        Back = backCommands.ToCollection().Select(x => x.LastOrDefault());
    }

    public IObservable<ReactiveCommand<Unit, Unit>?> Back { get; }
    public IObservable<bool> CanNavigateBack { get; }
}
