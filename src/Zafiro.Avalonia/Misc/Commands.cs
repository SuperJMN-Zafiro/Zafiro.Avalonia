using System.Windows.Input;
using Zafiro.Avalonia.Services;

namespace Zafiro.Avalonia.Misc;

public static class Commands
{
    public static readonly ICommand LaunchUri = ReactiveCommand.CreateFromTask<string>(str => LauncherService.Instance.LaunchUri(new Uri(str)));
}