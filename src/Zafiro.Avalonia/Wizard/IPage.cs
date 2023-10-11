using Zafiro.Avalonia.WizardOld.Interfaces;

namespace Zafiro.Avalonia.Wizard;

public interface IPage : IValidatable
{
    IValidatable Content { get; }
    string NextText { get;  }
    public string Title { get; }
}