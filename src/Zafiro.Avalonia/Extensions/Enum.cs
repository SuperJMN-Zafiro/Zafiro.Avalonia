using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.Extensions;

using System;

public class EnumValuesExtension : MarkupExtension
{
    public Type EnumType { get; set; } = null!;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(EnumType);
    }
}