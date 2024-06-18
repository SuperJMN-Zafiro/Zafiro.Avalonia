using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Avalonia.Dialogs.Simple;

namespace Zafiro.Avalonia.Dialogs;

public abstract class DialogService
{
    [PublicAPI]
    public static ISimpleDialog Create(IApplicationLifetime lifetime)
    {
        return lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => new SimpleDesktopDialogService(),
            ISingleViewApplicationLifetime { MainView: not null } singleView => throw new NotImplementedException("Not implemented yet"),
            ISingleViewApplicationLifetime { MainView: null } => throw new InvalidOperationException("Please, set MainView in the Application.Current.Lifetime before calling this method"),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };
    }
}