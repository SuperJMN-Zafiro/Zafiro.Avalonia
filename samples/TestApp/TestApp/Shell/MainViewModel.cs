using Zafiro.UI.Shell;

namespace TestApp.Shell;

public class MainViewModel(IShell shell)
{
    public IShell Shell { get; set; } = shell;
}