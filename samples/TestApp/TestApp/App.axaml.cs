using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using TestApp.ViewModels;
using TestApp.Views;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Services;
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
        var topLevel = TopLevel.GetTopLevel(view)!;
        var avaloniaFilePicker = new AvaloniaFileSystemPicker(topLevel.StorageProvider);
        INotificationService notificationService = new NotificationService(new WindowNotificationManager(topLevel));
        return new MainViewModel(DialogService.Create(), avaloniaFilePicker, notificationService);
    }
}