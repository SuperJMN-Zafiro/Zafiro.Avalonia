using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.Wizards;

public class WizardControl : TemplatedControl
{
    public static readonly StyledProperty<Wizard> WizardProperty = AvaloniaProperty.Register<WizardControl, Wizard>(
        nameof(Wizard));

    public Wizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}