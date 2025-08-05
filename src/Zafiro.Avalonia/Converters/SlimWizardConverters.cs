using Avalonia.Data.Converters;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Converters;

public static class SlimWizardConverters
{
    public static FuncValueConverter<StepKind, bool> CancelVisible { get; } = new(kind => kind != StepKind.Completion);
}

