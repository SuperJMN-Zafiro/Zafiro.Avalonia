using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace SampleFileExplorer;

public class SelectionModelBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Selection = new SelectionModel<object>();
        this.WhenAnyValue(behavior => behavior.SelectedItems).Subscribe(collection => { });
    }

    public static readonly StyledProperty<object> SelectedItemsProperty = AvaloniaProperty.Register<SelectionModelBehavior, object>(
        "SelectedItems");

    public object SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }
}