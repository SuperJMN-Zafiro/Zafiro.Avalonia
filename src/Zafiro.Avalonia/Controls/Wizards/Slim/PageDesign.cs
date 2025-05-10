using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public class PageDesign : IPage
{
    public object Content { get; } = "This is some content";
    public string Title { get; } = "Title";
    public int Index { get; } = 2;
}