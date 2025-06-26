using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogService
{
    public static IDialog Create()
    {
        if (Application.Current is null)
        {
            throw new InvalidOperationException("Application is not initialized.");
        }
        
        return Application.Current.ApplicationLifetime switch
        {
            ISingleViewApplicationLifetime singleViewApplicationLifetime => new AdornerDialog(() => GetAdornerLayer(singleViewApplicationLifetime)),
            _ => new StackedDialog()
        };
    }

    private static AdornerLayer GetAdornerLayer(ISingleViewApplicationLifetime lifetime)
    {
        var mainView = lifetime.MainView ?? throw new InvalidOperationException("Main view is not set.");
        return AdornerLayer.GetAdornerLayer(mainView) ?? throw new InvalidOperationException("Cannot get the adorner layer.");
    }
}