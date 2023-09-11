using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Dialogs;

public abstract class DialogService
{
    [PublicAPI]
    public static IDialogService Create(IApplicationLifetime lifetime, Maybe<Action<ConfigureWindowContext>> configureWindow)
    {
        return lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => new DesktopDialogService(configureWindow),
            ISingleViewApplicationLifetime { MainView: not null } singleView => new SingleViewDialogService(singleView.MainView),
            ISingleViewApplicationLifetime { MainView: null } => throw new InvalidOperationException("Please, set MainView in the Application.Current.Lifetime before calling this method"),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };
    }
}