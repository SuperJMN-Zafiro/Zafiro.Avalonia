using Avalonia.Controls.Primitives;
using Zafiro.UI.Wizards;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class SlimWizardContorl : TemplatedControl
{
    public static readonly StyledProperty<ISlimWizard> WizardProperty = AvaloniaProperty.Register<SlimWizardContorl, ISlimWizard>(
        nameof(Wizard));

    public ISlimWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }
}