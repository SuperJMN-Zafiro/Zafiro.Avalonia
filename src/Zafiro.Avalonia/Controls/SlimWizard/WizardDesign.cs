using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class WizardDesign : ISlimWizard
{
    public IEnhancedCommand Next { get; } = ReactiveCommand.Create(() => { }).Enhance();
    public IEnhancedCommand Back { get; } = ReactiveCommand.Create(() => { }).Enhance();
    public IPage CurrentPage { get; } = new PageDesign();
    public int TotalPages { get; } = 3;
}

public class PageDesign : IPage
{
    public object Content { get; } = "This is some content";
    public string Title { get; } = "Title";
    public int Index { get; } = 2;
}