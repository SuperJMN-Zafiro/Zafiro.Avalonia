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
    public string FriendlyName { get; set; }
    public object? Icon { get; set; }
    public IObservable<bool> IsVisible { get; init; } = Observable.Return(true);
    public IObservable<int> SortOrder { get; init; } = Observable.Return(0);
    public ICommand Command { get; } = ReactiveCommand.Create(() => { });
}