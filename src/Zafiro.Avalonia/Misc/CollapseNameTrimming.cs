using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Zafiro.Avalonia.Misc;

public class CollapseNameTrimming : TextTrimming
{
    public static CollapseNameTrimming Instance { get; } = new();

    public override TextCollapsingProperties CreateCollapsingProperties(TextCollapsingCreateInfo createInfo)
    {
        return new NameCollapsingProperties(createInfo.Width, createInfo.FlowDirection);
    }
}