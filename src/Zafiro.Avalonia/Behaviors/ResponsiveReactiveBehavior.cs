using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors
{
    public class ResponsiveReactiveBehavior : Behavior<Control>, IDisposable
    {
        // Debounce
        public static readonly StyledProperty<int> DebounceMillisecondsProperty =
            AvaloniaProperty.Register<ResponsiveReactiveBehavior, int>(
                nameof(DebounceMilliseconds), 50);

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private bool overflow;

        public int DebounceMilliseconds
        {
            get => GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        public void Dispose() => disposables.Dispose();

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject == null)
                return;

            // Observe layout updates or size changes
            var layoutUpdated = Observable.FromEventPattern(h => AssociatedObject.LayoutUpdated += h, h => AssociatedObject.LayoutUpdated -= h).ToSignal();
            var boundsChanged = AssociatedObject.GetObservable(Visual.BoundsProperty).ToSignal();

            var layoutChanged = layoutUpdated.Merge(boundsChanged);

            layoutChanged
                .ObserveOn(AvaloniaScheduler.Instance)
                .Throttle(TimeSpan.FromMilliseconds(DebounceMilliseconds), AvaloniaScheduler.Instance)
                .Subscribe(_ => UpdateState())
                .DisposeWith(disposables);
        }

        protected override void OnDetaching()
        {
            disposables.Dispose();
            base.OnDetaching();
        }

        private void UpdateState()
        {
            if (AssociatedObject == null)
                return;

            bool hasOverflow = CheckOverflow();
            if (hasOverflow == overflow)
                return;

            overflow = hasOverflow;
            ApplyState();
        }

        private bool CheckOverflow()
        {
            if (AssociatedObject is not Panel panel)
                return false;

            var bounds = AssociatedObject.Bounds;
            double total = 0;

            foreach (var child in panel.Children.OfType<Control>())
            {
                child.Measure(new Size(double.PositiveInfinity, bounds.Height));
                total += child.DesiredSize.Width;
                if (total > bounds.Width)
                    return true;
            }

            return false;
        }

        private void ApplyState()
        {
            // Style class
            var classes = (IPseudoClasses)AssociatedObject.Classes;
            if (overflow)
            {
                classes.Set(":overflow", true);
            }
            else
            {
                classes.Set(":overflow", false);
            }
        }
    }
}