using System.Windows.Input;

namespace Zafiro.Avalonia.Shell.Sections;

public class Section<T>(string name, Func<T> getViewModel, object? icon = null) : SectionBase, IContentSection
{
    public string Name { get; } = name;

    Func<object?> IContentSection.GetViewModel => () => GetViewModel();
    public Func<T> GetViewModel { get; } = getViewModel;

    public object? Icon { get; } = icon;

    public object? Content => GetViewModel();
}

public static class Section
{
    public static Section<T> Create<T>(string name, Func<T> getViewModel, object? icon = null, bool isPrimary = true)
    {
        return new Section<T>(name, getViewModel, icon)
        {
            IsPrimary = isPrimary,
        };
    }
    
    public static CommandSection Command(string name, ICommand command, object? icon = null, bool isPrimary = true)
    {
        return new CommandSection(name, command, icon)
        {
            IsPrimary = isPrimary,
        };
    }
    
    public static SectionSeparator Separator(bool isPrimary = true)
    {
        return new SectionSeparator
        {
            IsPrimary = isPrimary,
        };
    }
}