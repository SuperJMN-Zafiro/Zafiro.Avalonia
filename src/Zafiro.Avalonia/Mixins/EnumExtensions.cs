using System.ComponentModel;
using System.Reflection;

namespace Zafiro.Avalonia.Mixins;

public static class EnumMixin
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttr = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttr?.Description ?? value.ToString();
    }
}