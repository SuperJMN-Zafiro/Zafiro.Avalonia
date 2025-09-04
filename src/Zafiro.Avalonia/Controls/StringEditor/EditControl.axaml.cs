using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Zafiro.Avalonia.ViewLocators;

namespace Zafiro.Avalonia.Controls.StringEditor;

public class EditControl : TemplatedControl
{
    public static readonly StyledProperty<ReactiveCommandBase<Unit, Unit>> AcceptCommandProperty = AvaloniaProperty.Register<EditControl, ReactiveCommandBase<Unit, Unit>>(
        nameof(AcceptCommand));

    public static readonly DirectProperty<EditControl, bool> IsEditingProperty = AvaloniaProperty.RegisterDirect<EditControl, bool>(
        nameof(IsEditing), o => o.IsEditing, (o, v) => o.IsEditing = v);

    public static readonly StyledProperty<ReactiveCommandBase<Unit, Unit>> CancelCommandProperty = AvaloniaProperty.Register<EditControl, ReactiveCommandBase<Unit, Unit>>(
        nameof(CancelCommand));

    private readonly CompositeDisposable disposables = new();

    private bool isEditing;

    public ReactiveCommandBase<Unit, Unit> CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public ReactiveCommandBase<Unit, Unit> AcceptCommand
    {
        get => GetValue(AcceptCommandProperty);
        set => SetValue(AcceptCommandProperty, value);
    }

    public bool IsEditing
    {
        get => isEditing;
        protected set => SetAndRaise(IsEditingProperty, ref isEditing, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var editButton = (Button)e.NameScope.Find("PART_EditButton");
        var cancelButton = (Button)e.NameScope.Find("PART_CancelButton");
        var acceptButton = (Button)e.NameScope.Find("PART_AcceptButton");
        editButton?.OnEvent(Button.ClickEvent).Subscribe(_ => IsEditing = true).DisposeWith(disposables);
        cancelButton?.OnEvent(Button.ClickEvent).Subscribe(_ => IsEditing = false).DisposeWith(disposables);
        acceptButton?.OnEvent(Button.ClickEvent).Subscribe(_ => IsEditing = false).DisposeWith(disposables);
        base.OnApplyTemplate(e);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        disposables.Dispose();
    }
}