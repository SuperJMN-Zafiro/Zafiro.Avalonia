using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Primitives;
using DynamicData;
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

        var n = source
            .Connect()
            .AutoRefresh(x => x.AreDetailsShown)
            .AutoRefresh(x => x.IsCollapsed)
            .Filter(masterDetailsView => masterDetailsView is { AreDetailsShown: true, IsCollapsed: true });

        var commands = n.Transform(masterDetailsView => ReactiveCommand.Create(masterDetailsView.HideDetails));

        Backs = commands.ToCollection().Select(x => x.LastOrDefault());
    }

    public IObservable<ReactiveCommand<Unit, Unit>?> Backs { get; }
}
