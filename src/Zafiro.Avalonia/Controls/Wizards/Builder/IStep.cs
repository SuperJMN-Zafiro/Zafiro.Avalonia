namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public interface IStep : IValidatable, IBusy
{
    bool AutoAdvance { get; }
}