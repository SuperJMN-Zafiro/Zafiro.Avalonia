using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs;

public class DialogView : ContentControl
{
    public static readonly StyledProperty<double> OptionsPanelHeightProperty = AvaloniaProperty.Register<DialogView, double>(
        nameof(OptionsPanelHeight));

    public double OptionsPanelHeight
    {
        get => GetValue(OptionsPanelHeightProperty);
        set => SetValue(OptionsPanelHeightProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<IOption>> OptionsProperty = AvaloniaProperty.Register<DialogView, IEnumerable<IOption>>(
        nameof(Options));

    public IEnumerable<IOption> Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }
}