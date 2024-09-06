using System.Reactive.Disposables;
using Avalonia.Xaml.Interactions.Custom;
using Zafiro.Progress;
using Zafiro.UI.Jobs.Execution;

namespace Zafiro.Avalonia.Behaviors;

public class ProgressBarExecutionBehavior : DisposingBehavior<ProgressBar>
{
    public static readonly StyledProperty<IExecution> ExecutionProperty = AvaloniaProperty.Register<ProgressBarExecutionBehavior, IExecution>(nameof(Execution));

    public IExecution Execution
    {
        get => GetValue(ExecutionProperty);
        set => SetValue(ExecutionProperty, value);
    }

    protected override void OnAttached(CompositeDisposable disposables)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        this.WhenAnyObservable(x => x.Execution.Stop)
            .Do(_ => AssociatedObject.IsVisible = false)
            .Subscribe();

        this.WhenAnyObservable(x => x.Execution.Progress)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(progress =>
            {
                if (progress is NotStarted)
                {
                    AssociatedObject.IsVisible = false;
                }
                else
                {
                    AssociatedObject.IsVisible = true;
                }
                
                if (progress is ProportionalProgress ratioProgress)
                {
                    AssociatedObject.IsIndeterminate = false;
                    AssociatedObject.Maximum = 1;
                    AssociatedObject.Value = ratioProgress.Ratio;
                }

                if (progress is Completed)
                {
                    AssociatedObject.IsIndeterminate = false;
                    AssociatedObject.Value = AssociatedObject.Maximum;
                }

                if (progress is Unknown)
                {
                    AssociatedObject.IsIndeterminate = true;
                }
            })
            .Subscribe()
            .DisposeWith(disposables);
    }
}