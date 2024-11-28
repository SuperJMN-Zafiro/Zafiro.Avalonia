using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

public class TypewriterControl : TemplatedControl
{
    public static readonly StyledProperty<Strings> StringsProperty = AvaloniaProperty.Register<TypewriterControl, Strings>(
        nameof(Strings));

    public static readonly StyledProperty<TimeSpan> TypingLatencyProperty = AvaloniaProperty.Register<TypewriterControl, TimeSpan>(
        nameof(TypingLatency), TimeSpan.FromMilliseconds(100));

    public static readonly StyledProperty<TimeSpan> InBetweenPauseProperty = AvaloniaProperty.Register<TypewriterControl, TimeSpan>(
        nameof(InBetweenPause), TimeSpan.FromSeconds(3));

    public static readonly StyledProperty<IBrush> CaretBrushProperty = AvaloniaProperty.Register<TypewriterControl, IBrush>(
        nameof(CaretBrush));

    public static readonly StyledProperty<double> CaretWidthProperty = AvaloniaProperty.Register<TypewriterControl, double>(
        nameof(CaretWidth));

    public IBrush CaretBrush
    {
        get => GetValue(CaretBrushProperty);
        set => SetValue(CaretBrushProperty, value);
    }

    public double CaretWidth
    {
        get => GetValue(CaretWidthProperty);
        set => SetValue(CaretWidthProperty, value);
    }

    public TimeSpan InBetweenPause
    {
        get => GetValue(InBetweenPauseProperty);
        set => SetValue(InBetweenPauseProperty, value);
    }

    public TimeSpan TypingLatency
    {
        get => GetValue(TypingLatencyProperty);
        set => SetValue(TypingLatencyProperty, value);
    }

    public Strings Strings
    {
        get => GetValue(StringsProperty);
        set => SetValue(StringsProperty, value);
    }

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty = AvaloniaProperty.Register<TypewriterControl, TextWrapping>(
        nameof(TextWrapping));

    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }
}