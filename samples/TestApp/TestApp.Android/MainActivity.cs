using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;

namespace TestApp.Android;

[Activity(
    Label = "Zafiro.Avalonia",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/Icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var b = base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
#if DEBUG
        b = b.LogToTrace();
#endif
        return b;
    }
}