using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;

namespace MultiSelectorSample.Views;

public class MultiSelector : ItemsControl
{
    private bool canUpdate = true;

    public MultiSelector()
    {
        var changes = this
            .WhenAnyValue(x => x.Items)
            .WhereNotNull()
            .Select(x => x.Cast<ISelectableNotify>())
            .Select(x => x.AsObservableChangeSet())
            .Switch();

        var observable = changes
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(GetSelectionState);

        BehaviorSubject<bool?> subject = new BehaviorSubject<bool?>(false);
        observable.Subscribe(subject);

        IsChecked = observable
            .Where(_ => canUpdate)
            .Replay(1)
            .RefCount();

        Toggle = ReactiveCommand.CreateFromObservable(() => Do(changes, subject));
    }

    private IObservable<IReadOnlyCollection<ISelectableNotify>> Do(IObservable<IChangeSet<ISelectableNotify>> changes, BehaviorSubject<bool?> subject)
    {
        return changes
            .ToCollection()
            .Do(x =>
            {
                var toSet = !subject.Value ?? false;
                canUpdate = false;
                x.ToList().ForEach(notify => notify.IsSelected = toSet);
                canUpdate = true;
            })
            .Take(1);
    }

    public ReactiveCommand<Unit, IReadOnlyCollection<ISelectableNotify>> Toggle { get; }

    private static bool? GetSelectionState(IReadOnlyCollection<ISelectableNotify> collection)
    {
        return collection.All(x => x.IsSelected) ? true : collection.Any(x => x.IsSelected) ? (bool?) null : false;
    }

    public IObservable<bool?> IsChecked { get; }
}

public interface ISelectableNotify : INotifyPropertyChanged
{
    public bool IsSelected { get; set; }
}