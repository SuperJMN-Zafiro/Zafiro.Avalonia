using System.Reactive.Disposables;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors
{
    public sealed class OverflowBehavior : Behavior<Control>, IDisposable
    {
        private const double Hysteresis = 20.0;

        public static readonly StyledProperty<int> DebounceMillisecondsProperty =
            AvaloniaProperty.Register<OverflowBehavior, int>(
                nameof(DebounceMilliseconds), 50);

        private readonly CompositeDisposable disposables = new();
        private bool overflow;
        private bool isUpdating;

        public int DebounceMilliseconds
        {
            get => GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        public void Dispose() => disposables.Dispose();

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is null)
                return;

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
                .Throttle(TimeSpan.FromMilliseconds(DebounceMilliseconds))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => UpdateState());

            disposables.Add(layoutUpdated);

            // Initial check
            Dispatcher.UIThread.Post(UpdateState, DispatcherPriority.Background);
        }

        protected override void OnDetaching()
        {
            disposables.Dispose();
            base.OnDetaching();
        }

        private void UpdateState()
        {
            if (AssociatedObject is null || isUpdating)
                return;

            isUpdating = true;
            
            try
            {
                double total = MeasureTotalChildrenWidth();
                double width = AssociatedObject.Bounds.Width;

                // More conservative hysteresis logic
                bool newOverflow = overflow 
                    ? total > width - Hysteresis  // Keep overflow if still over lower threshold
                    : total > width + Hysteresis; // Set overflow only if significantly over

                if (newOverflow != overflow)
                {
                    overflow = newOverflow;
                    ApplyState();
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        private double MeasureTotalChildrenWidth()
        {
            if (AssociatedObject is null)
                return 0;

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