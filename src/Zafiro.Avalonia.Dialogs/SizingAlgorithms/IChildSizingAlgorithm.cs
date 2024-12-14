using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.Simple;

public interface IChildSizingAlgorithm
{
    public Size GetWindowSize(Control content,
        Size? screenSize = null,
        Size? parentWindowSize = null);
}