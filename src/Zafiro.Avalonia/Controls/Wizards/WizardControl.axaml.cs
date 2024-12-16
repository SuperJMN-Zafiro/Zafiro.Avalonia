using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.Wizards;

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