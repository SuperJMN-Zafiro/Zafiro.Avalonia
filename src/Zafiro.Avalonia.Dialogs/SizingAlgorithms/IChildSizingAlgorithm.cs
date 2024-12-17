using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.SizingAlgorithms;

public interface IChildSizingAlgorithm
{
    public Size GetWindowSize(Control content,
        Size? screenSize = null,
        Size? parentWindowSize = null);
}