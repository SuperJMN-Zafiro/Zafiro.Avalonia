using Avalonia.Controls.Primitives;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public class WizardNavigator : TemplatedControl
{
    public static readonly StyledProperty<ISlimWizard> WizardProperty = AvaloniaProperty.Register<WizardNavigator, ISlimWizard>(
        nameof(Wizard));

    public static readonly StyledProperty<IEnhancedCommand> CancelProperty = AvaloniaProperty.Register<WizardNavigator, IEnhancedCommand>(
        nameof(Cancel));

    public ISlimWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }

    public IEnhancedCommand Cancel
    {
        get => GetValue(CancelProperty);
        set => SetValue(CancelProperty, value);
    }
}