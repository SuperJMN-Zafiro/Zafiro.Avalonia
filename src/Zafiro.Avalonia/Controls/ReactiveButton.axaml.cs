using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls;

public class ReactiveButton : Button
{
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<ReactiveButton, object?>(nameof(Icon));

    public static readonly DirectProperty<ReactiveButton, bool> IsCommandRunningProperty =
        AvaloniaProperty.RegisterDirect<ReactiveButton, bool>(nameof(IsCommandRunning), button => button.isCommandRunning);

    readonly SerialDisposable commandExecutionSubscription = new();
    bool isCommandRunning;

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsCommandRunning
    {
        get => isCommandRunning;
        private set => SetAndRaise(IsCommandRunningProperty, ref isCommandRunning, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        commandExecutionSubscription.Disposable = ExecutionStates()
            .Subscribe(value => IsCommandRunning = value);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        commandExecutionSubscription.Disposable = Disposable.Empty;
    }

    IObservable<bool> ExecutionStates() => this
        .GetObservable(CommandProperty)
        .StartWith(Command)
        .Select(ObserveExecution)
        .Switch()
        .DistinctUntilChanged()
        .ObserveOn(RxApp.MainThreadScheduler);

    static IObservable<bool> ObserveExecution(ICommand? command) =>
        Maybe.From(command)
            .Bind(ToReactiveCommand)
            .Map(rc => rc.IsExecuting.StartWith(false))
            .GetValueOrDefault(Observable.Return(false));

    static Maybe<IReactiveCommand> ToReactiveCommand(ICommand command) =>
        command switch
        {
            IEnhancedCommand enhanced => Maybe.From((IReactiveCommand)enhanced),
            IReactiveCommand reactive => Maybe.From(reactive),
            _ => Maybe<IReactiveCommand>.None,
        };
}
