using System.ComponentModel;
using System.Reflection;

namespace Zafiro.Avalonia.Mixins;

// TODO: Move to Zafiro
public static class EnumMixin
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttr = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttr?.Description ?? value.ToString();
    }
}