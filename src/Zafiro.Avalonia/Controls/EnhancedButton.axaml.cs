using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Media;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls;

public class EnhancedButton : Button
{
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<EnhancedButton, object?>(nameof(Icon));

    public static readonly DirectProperty<EnhancedButton, bool> IsCommandRunningProperty = AvaloniaProperty.RegisterDirect<EnhancedButton, bool>(nameof(IsCommandRunning), button => button.isCommandRunning);

    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<EnhancedButton, double>(
        nameof(Spacing));

    public static readonly StyledProperty<Color> TintProperty = AvaloniaProperty.Register<EnhancedButton, Color>(
        nameof(Tint));

    readonly SerialDisposable commandExecutionSubscription = new();
    private bool isCommandRunning;

    public Color Tint
    {
        get => GetValue(TintProperty);
        set => SetValue(TintProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

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
        commandExecutionSubscription.Disposable = ExecutionStates().BindTo(this, x => x.IsCommandRunning);
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