using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.Views;

public class DialogView : ContentControl
{
    public static readonly StyledProperty<double> OptionsPanelHeightProperty = AvaloniaProperty.Register<DialogView, double>(
        nameof(OptionsPanelHeight));

    public static readonly StyledProperty<IEnumerable<IOption>> OptionsProperty = AvaloniaProperty.Register<DialogView, IEnumerable<IOption>>(
        nameof(Options));

    public double OptionsPanelHeight
    {
        get => GetValue(OptionsPanelHeightProperty);
        set => SetValue(OptionsPanelHeightProperty, value);
    }

    public IEnumerable<IOption> Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }
}