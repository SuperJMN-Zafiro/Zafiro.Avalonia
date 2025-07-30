using System.ComponentModel;
using System.Reflection;

namespace Zafiro.Avalonia.Misc;

// TODO: Move to Zafiro
public static class EnumMixin
{
    public static string GetDescription(this System.Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttr = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttr?.Description ?? value.ToString();
    }
}