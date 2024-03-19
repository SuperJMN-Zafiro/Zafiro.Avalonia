using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.Mixins;

internal static class Extensions
{
    public static Uri? GetContextBaseUri(this IServiceProvider ctx)
    {
        return ctx.GetService<IUriContext>()?.BaseUri;
    }

    private static T? GetService<T>(this IServiceProvider sp)
    {
        return (T?)sp.GetService(typeof(T));
    }
}