using Avalonia.Controls.Primitives;
using Zafiro.UI.Wizards.Classic;

namespace Zafiro.Avalonia.Controls.Wizards.Classic;

public class WizardControl : TemplatedControl
{
    public static readonly StyledProperty<IWizard> WizardProperty = AvaloniaProperty.Register<WizardControl, IWizard>(
        nameof(Wizard));

    public IWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}