using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Zafiro.Avalonia.Dialogs.Implementations;

namespace Zafiro.Avalonia.Dialogs;

/// <summary>
/// Factory for creating dialog instances appropriate for the current application lifetime.
/// </summary>
public static class DialogService
{
    /// <summary>
    /// Creates a dialog implementation suitable for the current application lifetime.
    /// Uses adaptive sizing for optimal user experience.
    /// </summary>
    /// <param name="sizingConfig">Optional sizing configuration.</param>
    /// <param name="sizingStrategy">Optional custom sizing strategy.</param>
    /// <returns>An IDialog implementation.</returns>
    public static IDialog Create(
        AdaptiveDialogSizer.SizingConfig? sizingConfig = null,
        IDialogSizingStrategy? sizingStrategy = null)
    {
        if (Application.Current is null)
        {
            throw new InvalidOperationException("Application is not initialized.");
        }

        return Application.Current.ApplicationLifetime switch
        {
            ISingleViewApplicationLifetime singleViewApplicationLifetime =>
                new AdaptiveAdornerDialog(
                    () => GetAdornerLayer(singleViewApplicationLifetime),
                    sizingConfig,
                    sizingStrategy),
            _ => new DesktopDialog()
        };
    }

    private static AdornerLayer GetAdornerLayer(ISingleViewApplicationLifetime lifetime)
    {
        var mainView = lifetime.MainView ?? throw new InvalidOperationException("Main view is not set.");
        return AdornerLayer.GetAdornerLayer(mainView) ?? throw new InvalidOperationException("Cannot get the adorner layer.");
    }
}