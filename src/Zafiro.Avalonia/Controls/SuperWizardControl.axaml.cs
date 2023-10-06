using Avalonia;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls;

public class SuperWizardControl : TemplatedControl
{
    public static readonly StyledProperty<ISuperWizard?> WizardProperty = AvaloniaProperty.Register<WizardControl, ISuperWizard?>(
        nameof(Wizard));

    public ISuperWizard? Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}