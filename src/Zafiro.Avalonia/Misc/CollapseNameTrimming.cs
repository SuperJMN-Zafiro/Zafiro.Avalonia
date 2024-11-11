using Avalonia.Media.TextFormatting;
using Avalonia.Media;

namespace Zafiro.Avalonia.Misc;

public class CollapseNameTrimming : TextTrimming
{
    public static CollapseNameTrimming Instance { get; } = new();

    public override TextCollapsingProperties CreateCollapsingProperties(TextCollapsingCreateInfo createInfo)
    {
        return new NameCollapsingProperties(createInfo.Width, createInfo.FlowDirection);
    }
}