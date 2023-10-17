using Zafiro.Avalonia.Model;

namespace Zafiro.Avalonia.Wizard;

public class Page<T> : IPage where T : IValidatable
{
    public Page(T content, string next, string title)
    {
        Content = content;
        NextText = next;
        Title = title;
    }

    public string NextText { get; set; }
    public string Title { get; }
    public IValidatable Content { get; }
    public IObservable<bool> IsValid => Content.IsValid;
}