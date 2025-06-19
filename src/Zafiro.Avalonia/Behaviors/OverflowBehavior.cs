using System.Reactive.Disposables;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors
{
    public sealed class OverflowBehavior : Behavior<Control>, IDisposable
    {
        private const double Hysteresis = 20.0;

        public static readonly StyledProperty<int> DebounceMillisecondsProperty =
            AvaloniaProperty.Register<OverflowBehavior, int>(
                nameof(DebounceMilliseconds), 50);

        private readonly SerialDisposable subscription = new();
        private bool overflow;

        public int DebounceMilliseconds
        {
            get => GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        public void Dispose() => subscription.Dispose();

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is null)
                return;

            var cd = new CompositeDisposable();
            subscription.Disposable = cd;

            var layoutUpdated = Observable
                .FromEventPattern(
                    h => AssociatedObject.LayoutUpdated += h,
                    h => AssociatedObject.LayoutUpdated -= h)
                .Select(_ => AssociatedObject.Bounds)
                .DistinctUntilChanged()
                .ToSignal();

            var layoutDisp = layoutUpdated
                .ObserveOn(AvaloniaScheduler.Instance)
                .Throttle(
                    TimeSpan.FromMilliseconds(DebounceMilliseconds),
                    AvaloniaScheduler.Instance)
                .Subscribe(_ => UpdateState());

            cd.Add(layoutDisp);

            // Primera evaluación tras el pase de render en cola baja
            Dispatcher.UIThread.Post(UpdateState, DispatcherPriority.Background);
        }

        protected override void OnDetaching()
        {
            subscription.Disposable?.Dispose();
            base.OnDetaching();
        }

        private async void UpdateState()
        {
            if (AssociatedObject is null)
                return;

            // Esperar al pase de render antes de medir
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);

            double total = MeasureTotalChildrenWidth();
            double width = AssociatedObject.Bounds.Width;

            bool newOverflow = !overflow
                ? total > width + Hysteresis
                : total >= width - Hysteresis;

            if (newOverflow == overflow)
                return;

            overflow = newOverflow;
            ApplyState();
        }

        private double MeasureTotalChildrenWidth()
        {
            var panel = AssociatedObject switch
            {
                ItemsControl ic => ic.ItemsPanelRoot,
                Panel p => p,
                _ => null
            };
            if (panel is null)
                return 0;

            double sum = 0;
            foreach (Control child in panel.Children.OfType<Control>())
            {
                child.Measure(new Size(double.PositiveInfinity, panel.Bounds.Height));
                sum += child.DesiredSize.Width;
            }

            return sum;
        }

        private void ApplyState()
        {
            IPseudoClasses? pc = AssociatedObject?.Classes;
            pc?.Set(":overflow", overflow);
        }
    }
}