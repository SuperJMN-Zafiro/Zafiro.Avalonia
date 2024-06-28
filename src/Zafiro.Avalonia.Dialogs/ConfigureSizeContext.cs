using Avalonia;

namespace Zafiro.Avalonia.Dialogs;

public record ConfigureSizeContext
{
    public double Height { get; set; }
    public double Width { get; set; }
    public Rect ParentBounds { get; init; }
}