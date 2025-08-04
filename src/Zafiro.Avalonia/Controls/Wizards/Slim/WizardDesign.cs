using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public class WizardDesign : ISlimWizard
{
    public IEnhancedCommand Next { get; } = ReactiveCommand.Create(() => { }).Enhance("Next");
    public IEnhancedCommand Back { get; } = ReactiveCommand.Create(() => { }).Enhance();
    public IPage CurrentPage { get; } = new PageDesign();
    public int TotalPages { get; } = 3;
}