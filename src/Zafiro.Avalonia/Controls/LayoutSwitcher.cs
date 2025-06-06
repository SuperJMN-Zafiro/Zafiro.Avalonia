using Avalonia.Controls.Presenters;

namespace Zafiro.Avalonia.Controls
{
    public class LayoutSwitcher : Panel
    {
        private const double SwitchTolerance = 5.0;

        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<LayoutSwitcher, object?>(nameof(Content));

        public static readonly StyledProperty<object?> OverflowContentProperty =
            AvaloniaProperty.Register<LayoutSwitcher, object?>(nameof(OverflowContent));

        public static readonly StyledProperty<bool> IsOverflowProperty =
            AvaloniaProperty.Register<LayoutSwitcher, bool>(nameof(IsOverflow));

        private Control? _contentControl;
        private Size _contentDesiredSize = new Size();
        private Size _lastMeasuredSize = new Size();
        private bool _lastOverflowState;
        private Control? _overflowControl;

        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public object? OverflowContent
        {
            get => GetValue(OverflowContentProperty);
            set => SetValue(OverflowContentProperty, value);
        }

        public bool IsOverflow
        {
            get => GetValue(IsOverflowProperty);
            private set => SetValue(IsOverflowProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ContentProperty || change.Property == OverflowContentProperty)
            {
                UpdateContent();
                InvalidateMeasure();
            }
        }

        private void UpdateContent()
        {
            Children.Clear();

            _contentControl = CreateControlFromContent(Content);
            _overflowControl = CreateControlFromContent(OverflowContent);

            if (_contentControl != null)
                Children.Add(_contentControl);

            if (_overflowControl != null)
                Children.Add(_overflowControl);

            // Reset cached sizes
            _contentDesiredSize = new Size();
        }

        private Control? CreateControlFromContent(object? content)
        {
            return content switch
            {
                null => null,
                Control control => control,
                string text => new TextBlock { Text = text },
                _ => new ContentPresenter { Content = content }
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Only measure content control once to get its natural size
            if (_contentControl != null && _contentDesiredSize.Width == 0 && _contentDesiredSize.Height == 0)
            {
                // Temporarily make it visible to measure, but don't affect layout decision
                var wasVisible = _contentControl.IsVisible;
                _contentControl.IsVisible = true;
                _contentControl.Measure(Size.Infinity);
                _contentDesiredSize = _contentControl.DesiredSize;
                _contentControl.IsVisible = wasVisible;
            }

            // Determine which layout to use based on available space and hysteresis
            bool shouldOverflow = ShouldUseOverflow(availableSize);

            // Only update if size changed significantly or state should change
            if (Math.Abs(_lastMeasuredSize.Width - availableSize.Width) > SwitchTolerance ||
                shouldOverflow != _lastOverflowState)
            {
                IsOverflow = shouldOverflow;
                _lastOverflowState = shouldOverflow;
                _lastMeasuredSize = availableSize;
            }

            // Measure and return size of active control
            if (IsOverflow && _overflowControl != null)
            {
                _overflowControl.IsVisible = true;
                _overflowControl.Measure(availableSize);
                return _overflowControl.DesiredSize;
            }
            else if (_contentControl != null)
            {
                // Use cached size to avoid re-measuring
                return _contentDesiredSize;
            }

            return new Size();
        }

        private bool ShouldUseOverflow(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || (_contentDesiredSize.Width == 0 && _contentDesiredSize.Height == 0))
                return false;

            // Apply hysteresis
            if (_lastOverflowState)
            {
                // Currently in overflow - switch back only if content fits comfortably
                return _contentDesiredSize.Width > (availableSize.Width + SwitchTolerance);
            }
            else
            {
                // Currently showing content - switch to overflow if content doesn't fit
                return _contentDesiredSize.Width > (availableSize.Width - SwitchTolerance);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Set visibility based on current state
            if (_contentControl != null)
                _contentControl.IsVisible = !IsOverflow;

            if (_overflowControl != null)
                _overflowControl.IsVisible = IsOverflow;

            // Arrange only the visible control
            if (IsOverflow && _overflowControl != null && _overflowControl.IsVisible)
            {
                _overflowControl.Arrange(new Rect(finalSize));
            }
            else if (!IsOverflow && _contentControl != null && _contentControl.IsVisible)
            {
                _contentControl.Arrange(new Rect(finalSize));
            }

            return finalSize;
        }
    }
}

// Usage example in XAML:
/*
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="using:Zafiro.Avalonia.Controls"
        x:Class="YourApp.MainWindow">

    <Grid RowDefinitions="Auto,*">
        <controls:LayoutSwitcher Grid.Row="0"
                                Background="LightGray"
                                Margin="10">
            <controls:LayoutSwitcher.Content>
                <StackPanel Orientation="Horizontal" Spacing="10">
                    <Button Content="Home" />
                    <Button Content="Products" />
                    <Button Content="Services" />
                    <Button Content="About Us" />
                    <Button Content="Contact" />
                </StackPanel>
            </controls:LayoutSwitcher.Content>

            <controls:LayoutSwitcher.OverflowContent>
                <StackPanel Orientation="Horizontal" Spacing="5">
                    <Button Content="☰" Width="30" />
                    <TextBlock Text="Menu" VerticalAlignment="Center" />
                </StackPanel>
            </controls:LayoutSwitcher.OverflowContent>
        </controls:LayoutSwitcher>

        <TextBlock Grid.Row="1"
                   Text="Resize window to see layout switching"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Grid>
</Window>
*/