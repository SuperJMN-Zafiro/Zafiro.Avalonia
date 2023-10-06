using Avalonia;
using Avalonia.Controls.Primitives;
using Zafiro.Avalonia.WizardOld.Interfaces;

namespace Zafiro.Avalonia.Controls;

public class OldWizardControl : TemplatedControl
{
    public static readonly StyledProperty<IWizard?> WizardProperty = AvaloniaProperty.Register<OldWizardControl, IWizard?>(
        nameof(Wizard));

    public IWizard? Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}