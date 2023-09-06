using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Model;

public interface IPage
{
    public IValidatable? Content { get; }
    void CreateContent(object? page);
    public Maybe<string> NextText { get; }
}