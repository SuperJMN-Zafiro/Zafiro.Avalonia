using Avalonia.Data.Converters;
using Zafiro.Avalonia.Controls.Wizards.Slim;

namespace Zafiro.Avalonia.Converters;

public static class DialogConverters
{
    public static readonly FuncValueConverter<object?, bool> IsSlimWizardControl = new(obj => obj is SlimWizardControl);
}
