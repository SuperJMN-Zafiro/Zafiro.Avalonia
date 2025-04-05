
namespace Zafiro.Avalonia.Controls.StatusBar.NotificationTypes;

public class PathContent : ReactiveObject
{
    public string ResourceName { get; }

    public PathContent(string resourceName)
    {
        ResourceName = resourceName;
    }
}