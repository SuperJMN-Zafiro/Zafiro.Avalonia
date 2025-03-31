namespace Zafiro.Avalonia.Shell.Sections;

public interface IContentSection
{
    string Name { get; }
    Func<object?> GetViewModel { get; }
    object? Icon { get; }
    object? Content { get; }
}