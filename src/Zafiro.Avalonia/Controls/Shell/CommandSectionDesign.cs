using System.Windows.Input;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class CommandSectionDesign : ReactiveObject, ICommandSection
{
    public bool IsPrimary { get; init; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public object? Icon { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public ICommand Command { get; } = ReactiveCommand.Create(() => { });
}