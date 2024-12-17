namespace Zafiro.Avalonia.Services;

public interface ILauncherService
{
    Task LaunchUri(Uri uri);
}