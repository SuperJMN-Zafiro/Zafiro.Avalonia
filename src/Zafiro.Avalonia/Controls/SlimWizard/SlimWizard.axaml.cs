using Avalonia.Controls.Primitives;
using Zafiro.UI.Wizard;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class SlimWizard : TemplatedControl
{
    public static readonly StyledProperty<IWizard> WizardProperty = AvaloniaProperty.Register<SlimWizard, IWizard>(
        nameof(Wizard));

    public IWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}