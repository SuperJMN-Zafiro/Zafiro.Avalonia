using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Services;

namespace Zafiro.Avalonia.Misc;

public class Link
{
    public string Url { get; }

    public object Icon { get; set; }

    public Link(ILauncherService lanLauncherService, string url)
    {
        Url = url;
        Open = ReactiveCommand.CreateFromTask(() => Result.Try(() => lanLauncherService.LaunchUri(new Uri(url))));
    }

    public ReactiveCommand<Unit, Result> Open { get;  }
}