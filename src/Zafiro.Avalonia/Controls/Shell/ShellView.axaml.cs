using Avalonia.Animation;
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

    public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty = AvaloniaProperty.Register<ShellView, IDataTemplate>(
        nameof(HeaderContentTemplate));

    public static readonly StyledProperty<double> MenuButtonSizeProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(MenuButtonSize));

    public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<ShellView, bool>(
        nameof(IsPaneOpen));

    public static readonly StyledProperty<IPageTransition> PageTransitionProperty = AvaloniaProperty.Register<ShellView, IPageTransition>(
        nameof(PageTransition));

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

    public IDataTemplate HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }

    public double MenuButtonSize
    {
        get => GetValue(MenuButtonSizeProperty);
        set => SetValue(MenuButtonSizeProperty, value);
    }

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public IPageTransition PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }
}