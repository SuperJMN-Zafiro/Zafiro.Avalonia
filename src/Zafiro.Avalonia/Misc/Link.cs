using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Misc;

public class Link
{
    public string Url { get; }

    public object Icon { get; set; }

    public Link(string url)
    {
        Url = url;
        Open = Commands.Instance.LaunchUri;
    }

    public ReactiveCommandBase<string, Result> Open { get;  }
}