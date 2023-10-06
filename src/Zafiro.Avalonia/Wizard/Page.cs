using Zafiro.Avalonia.WizardOld.Interfaces;

namespace Zafiro.Avalonia.Wizard;

public class Page<T> : IPage where T : IValidatable
{
    public Page(T content, string next)
    {
        Content = content;
        NextText = next;
    }

    public string NextText { get; set; }

    public IValidatable Content { get; }
    public IObservable<bool> IsValid => Content.IsValid;
}