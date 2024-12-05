using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls;

public class CircularProgressBar : TemplatedControl
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Value));

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Minimum));

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Maximum), 100.0);

    public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<CircularProgressBar, double>(
        nameof(StrokeThickness), 5d);

    public static readonly DirectProperty<CircularProgressBar, double> SizeProperty = AvaloniaProperty.RegisterDirect<CircularProgressBar, double>(
        nameof(Size), o => o.Size, (o, v) => o.Size = v);

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty = AvaloniaProperty.Register<CircularProgressBar, TimeSpan>(
        nameof(AnimationDuration), TimeSpan.FromSeconds(1));

    public static readonly StyledProperty<Easing> AnimationEasingProperty = AvaloniaProperty.Register<CircularProgressBar, Easing>(
        nameof(AnimationEasing), new CubicEaseOut());

    private double size;

    public CircularProgressBar()
    {
        this.WhenAnyValue(x => x.Bounds).Select(x => Math.Min(x.Height, x.Width)).BindTo(this, x => x.Size);
    }

    public Easing AnimationEasing
    {
        get => GetValue(AnimationEasingProperty);
        set => SetValue(AnimationEasingProperty, value);
    }

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public double Size
    {
        get => size;
        private set => SetAndRaise(SizeProperty, ref size, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
}