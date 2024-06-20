using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls.Selection;
using DynamicData;
using Microsoft.Reactive.Testing;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Tests;

public class ObservableSelectionModelTests
{
    [Fact]
    public void Empty_selection_sends_initial_empty_change()
    {
        var testScheduler = new TestScheduler();
        
        var selectionModel = new SelectionModel<Model>()
        {
            SingleSelect = false,
            Source = new[] { new Model("uno"), new Model("dos"), new Model("tres") }
        };
        var selectionTracker = new ObservableSelectionModel<Model, string>(selectionModel, x => x.Id);
        var observer = testScheduler.CreateObserver<IChangeSet<Model, string>>();
        selectionTracker.Selection.Subscribe(observer);
        
        var notifications = observer.Messages;

        Assert.Single(notifications);
        Assert.Equal(0, notifications[0].Value.Value.Count);
    }
    
    [Fact]
    public void Adding_item_sends_correct_add_update()
    {
        var testScheduler = new TestScheduler();
        
        var selectionModel = new SelectionModel<Model>()
        {
            SingleSelect = false,
            Source = new[] { new Model("uno"), new Model("dos"), new Model("tres") }
        };
        var selectionTracker = new ObservableSelectionModel<Model, string>(selectionModel, x => x.Id);
        
        var observer = testScheduler.CreateObserver<IChangeSet<Model, string>>();
        selectionTracker.Selection.Subscribe(observer);
        
        selectionModel.SelectAll();

        Assert.Equal(3, observer.Messages[1].Value.Value.Adds);
    }
}