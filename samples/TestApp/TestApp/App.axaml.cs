using Avalonia;
using Avalonia.Markup.Xaml;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Projektanker.Icons.Avalonia.MaterialDesign;
using TestApp.Shell;
using Zafiro.Avalonia.Misc;

namespace TestApp;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>()
            .Register<MaterialDesignIconProvider>();

        this.Connect(() => new MainView(), view => CompositionRoot.Create(), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }

    // private static MainViewModel MainViewModel(Control view)
    // {
    //     var topLevel = TopLevel.GetTopLevel(view)!;
    //     var avaloniaFilePicker = new AvaloniaFileSystemPicker(topLevel.StorageProvider);
    //     INotificationService notificationService = new NotificationService(new WindowNotificationManager(topLevel));
    //     return new MainViewModel(DialogService.Create(), avaloniaFilePicker, notificationService);
    // }
}