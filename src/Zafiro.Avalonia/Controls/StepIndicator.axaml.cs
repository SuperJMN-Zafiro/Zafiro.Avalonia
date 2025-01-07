using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class StepIndicator : TemplatedControl
{
    public static readonly StyledProperty<int> CurrentProperty = AvaloniaProperty.Register<StepIndicator, int>(
        nameof(Current));

    public int Current
    {
        get => GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    public static readonly StyledProperty<int> TotalProperty = AvaloniaProperty.Register<StepIndicator, int>(
        nameof(Total));

    public int Total
    {
        get => GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }
}