using Avalonia;
using Avalonia.Controls.Primitives;
using Zafiro.Avalonia.Model;

namespace Zafiro.Avalonia.Controls;

public class WizardControl : TemplatedControl
{
    public static readonly StyledProperty<Wizard?> WizardProperty = AvaloniaProperty.Register<WizardControl, Wizard?>(
        nameof(Wizard));

    public Wizard? Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}