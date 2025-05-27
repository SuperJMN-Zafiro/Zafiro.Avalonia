using Avalonia.Controls.Primitives;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Controls.Shell;

public class ShellView : TemplatedControl
{
    public static readonly StyledProperty<IShell> ShellProperty = AvaloniaProperty.Register<ShellView, IShell>(
        nameof(Shell));

    public static readonly StyledProperty<double> OpenPaneLengthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(OpenPaneLength));

    public static readonly StyledProperty<object> ContentHeaderProperty = AvaloniaProperty.Register<ShellView, object>(
        nameof(ContentHeader));

    public static readonly StyledProperty<double> CompactPaneLengthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(CompactPaneLength));

    public IShell Shell
    {
        get => GetValue(ShellProperty);
        set => SetValue(ShellProperty, value);
    }

    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    public object ContentHeader
    {
        get => GetValue(ContentHeaderProperty);
        set => SetValue(ContentHeaderProperty, value);
    }

    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }
}