using Avalonia.Controls.Primitives;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Shell;

public class ShellView : TemplatedControl
{
    public static readonly StyledProperty<IShell> ShellProperty = AvaloniaProperty.Register<ShellView, IShell>(
        nameof(Shell));

    public IShell Shell
    {
        get => GetValue(ShellProperty);
        set => SetValue(ShellProperty, value);
    }
}