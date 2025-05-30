using Avalonia.Markup.Xaml;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.Mixins;
using Enum = System.Enum;

namespace Zafiro.Avalonia.Extensions;

public class EnumDescriptionsExtension : MarkupExtension
{
    public Type EnumType { get; set; } = null!;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (!EnumType.IsEnum)
            throw new ArgumentException("EnumType must be an enum.");

        // Retorna una lista de objetos con Value (el enum) y Description (el texto a mostrar).
        var values = Enum.GetValues(EnumType)
            .Cast<Enum>()
            .Select(e => new EnumItem(e, e.GetDescription()))
            .ToList();
        return values;
    }
}