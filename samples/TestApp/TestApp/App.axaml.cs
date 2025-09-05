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

        // On Android, assign MainView and DataContext immediately to avoid a blank screen if Loaded is delayed
        if (ApplicationLifetime is global::Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime singleView)
        {
            var mainView = new MainView();
            singleView.MainView = mainView;

            // Log assembly resolution requests to identify missing assemblies under AOT
            System.AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                try
                {
                    var an = new System.Reflection.AssemblyName(args.Name);
                    System.Console.WriteLine($"[TestApp] AssemblyResolve requested: {an.Name}");
                }
                catch { }
                return null;
            };

            this.Connect(() => new MainView(), view => CompositionRoot.Create(), () => new MainWindow());
        }

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