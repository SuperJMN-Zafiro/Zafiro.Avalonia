using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using CSharpFunctionalExtensions;
using TestApp.ViewModels;
using TestApp.Views;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;
using Zafiro.Avalonia.Storage;
using Zafiro.UI;

namespace TestApp;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), view => MainViewModel(view), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }

    private static MainViewModel MainViewModel(Control view)
    {
        var dialogService = DialogService.Create(Current!.ApplicationLifetime!, Maybe<Action<ConfigureWindowContext>>.From(context => context.ToConfigure.SizeToContent = SizeToContent.WidthAndHeight));
        // Enable if you want to force the Single Dialog Service
        //var dialogService = new SingleViewDialogService(view);

        var topLevel = TopLevel.GetTopLevel(view)!;
        var avaloniaFilePicker = new AvaloniaFilePicker(topLevel.StorageProvider);
        INotificationService notificationService = new NotificationService(new WindowNotificationManager(topLevel));
        return new MainViewModel(dialogService, avaloniaFilePicker, notificationService);
    }
}