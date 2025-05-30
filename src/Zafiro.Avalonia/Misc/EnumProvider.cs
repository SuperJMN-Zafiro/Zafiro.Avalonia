using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Extensions;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Misc;

public partial class EnumProvider : ReactiveObject
{
    [ObservableAsProperty] private IList<EnumItem>? enums;

    [Reactive] private Type? enumType;

    public EnumProvider()
    {
        enumsHelper = this.WhenAnyValue<EnumProvider, Type?>(provider => provider.EnumType)
            .WhereNotNull()
            .Select(GetEnums)
            .ToProperty(this, provider => provider.Enums);
    }

    private IList<EnumItem> GetEnums(Type type)
    {
        var values = System.Enum.GetValues(EnumType!)
            .Cast<System.Enum>()
            .Select(e => new EnumItem(e, e.GetDescription()))
            .ToList();

        return values;
    }
}