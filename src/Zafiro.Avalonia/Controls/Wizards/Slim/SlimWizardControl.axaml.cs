using Avalonia.Controls.Primitives;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public class SlimWizardControl : TemplatedControl
{
    public static readonly StyledProperty<ISlimWizard> WizardProperty = AvaloniaProperty.Register<SlimWizardControl, ISlimWizard>(
        nameof(Wizard));

    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<SlimWizardControl, object?>(
        nameof(Header));

    public ISlimWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}