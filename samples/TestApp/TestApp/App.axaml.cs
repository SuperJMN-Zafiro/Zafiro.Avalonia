using Avalonia;
using Avalonia.Markup.Xaml;
using TestApp.ViewModels;
using TestApp.Views;
using Zafiro.Avalonia.Mixins;

namespace TestApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), _ => new MainViewModel(), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}