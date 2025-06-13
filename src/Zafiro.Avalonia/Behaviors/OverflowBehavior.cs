using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors
{
    public sealed class OverflowBehavior : Behavior<Control>
    {
        private const double Hysteresis = 10.0;

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

            if (AssociatedObject == null)
                return;

            // Renew subscriptions for this attach
            var cd = new CompositeDisposable();
            subscription.Disposable = cd;

            // Observe layout updates safely
            var layoutUpdated = Observable
                .FromEventPattern(
                    h => AssociatedObject.LayoutUpdated += h,
                    h =>
                    {
                        if (AssociatedObject != null)
                        {
                            AssociatedObject.LayoutUpdated -= h;
                        }
                    })
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

            // Initial evaluation
            UpdateState();
        }

        protected override void OnDetaching()
        {
            // Dispose current subscriptions
            subscription.Disposable?.Dispose();

            base.OnDetaching();
        }

        private void UpdateState()
        {
            if (AssociatedObject == null)
                return;

            double total = MeasureTotalChildrenWidth();
            double width = AssociatedObject.Bounds.Width;

            bool hasOverflow;
            if (!overflow)
                hasOverflow = total > width + Hysteresis;
            else
                hasOverflow = total >= width - Hysteresis;

            if (hasOverflow == overflow)
                return;

            overflow = hasOverflow;
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
            if (panel == null)
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