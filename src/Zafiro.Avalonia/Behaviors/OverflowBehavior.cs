using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors
{
    public class OverflowBehavior : Behavior<Control>, IDisposable
    {
        private const double HYSTERESIS = 10.0;

        public static readonly StyledProperty<int> DebounceMillisecondsProperty =
            AvaloniaProperty.Register<OverflowBehavior, int>(
                nameof(DebounceMilliseconds), 50);

        private readonly CompositeDisposable _disposables = new();
        private bool _overflow;

        public int DebounceMilliseconds
        {
            get => GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        public void Dispose() => _disposables.Dispose();

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null) return;

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

            layoutUpdated
                .ObserveOn(AvaloniaScheduler.Instance)
                .Throttle(TimeSpan.FromMilliseconds(DebounceMilliseconds), AvaloniaScheduler.Instance)
                .Subscribe(_ => UpdateState())
                .DisposeWith(_disposables);
        }

        protected override void OnDetaching()
        {
            _disposables.Dispose();
            base.OnDetaching();
        }

        private void UpdateState()
        {
            if (AssociatedObject == null)
                return;

            double total = MeasureTotalChildrenWidth();
            double width = AssociatedObject.Bounds.Width;

            bool hasOverflow;
            if (!_overflow)
                hasOverflow = total > width + HYSTERESIS;
            else
                hasOverflow = total >= width - HYSTERESIS;

            if (hasOverflow == _overflow)
                return;

            _overflow = hasOverflow;
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
            var pc = (IPseudoClasses)AssociatedObject.Classes;
            pc.Set(":overflow", _overflow);
        }
    }
}