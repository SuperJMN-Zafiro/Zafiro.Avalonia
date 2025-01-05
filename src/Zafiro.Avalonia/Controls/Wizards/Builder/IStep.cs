using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public interface IStep : IValidatable, IBusy
{
    bool AutoAdvance { get; }
    public Maybe<string> Title => Maybe<string>.None;
}