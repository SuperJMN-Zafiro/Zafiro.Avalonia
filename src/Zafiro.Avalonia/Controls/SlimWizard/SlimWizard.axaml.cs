using Avalonia.Controls.Primitives;
using Zafiro.UI.Wizards;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class SlimWizard : TemplatedControl
{
    public static readonly StyledProperty<INewWizard> WizardProperty = AvaloniaProperty.Register<SlimWizard, INewWizard>(
        nameof(Wizard));

    public INewWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}