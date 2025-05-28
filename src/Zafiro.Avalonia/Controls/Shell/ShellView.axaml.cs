using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Controls.Shell;

public class ShellView : TemplatedControl
{
    public static readonly StyledProperty<IShell> ShellProperty = AvaloniaProperty.Register<ShellView, IShell>(
        nameof(Shell));

    public static readonly StyledProperty<double> OpenPaneLengthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(OpenPaneLength));

    public static readonly StyledProperty<double> CompactPaneLengthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(CompactPaneLength));

    public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty = AvaloniaProperty.Register<ShellView, IDataTemplate>(
        nameof(HeaderContentTemplate));

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

    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }

    public IDataTemplate HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }
}