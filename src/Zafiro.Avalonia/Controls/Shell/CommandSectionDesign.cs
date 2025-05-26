using System.Windows.Input;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class CommandSectionDesign : ICommandSection
{
    public CommandSectionDesign()
    {
    }

    public CommandSectionDesign(string name, object? icon = null, bool isPrimary = true)
    {
        Name = name;
        Icon = icon;
        IsPrimary = isPrimary;
    }
    
    public bool IsPrimary { get; init; }
    public string Name { get; set; }
    public object? Icon { get; set; }
    public ICommand Command { get; }
}