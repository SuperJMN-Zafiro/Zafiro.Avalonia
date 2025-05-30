using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Zafiro.Avalonia.Behaviors;

public static class AvaloniaParseHelper
{
    private static readonly ConcurrentDictionary<Type, MethodInfo?> ParseMethods = new();

    public static object? InvokeParse(string s, Type targetType)
    {
        if (s is null) throw new ArgumentNullException(nameof(s));
        if (targetType is null) throw new ArgumentNullException(nameof(targetType));

        var method = ParseMethods.GetOrAdd(targetType, t =>
        {
            var m = t.GetMethod(
                "Parse",
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(string), typeof(CultureInfo)],
                null);
            return m ?? t.GetMethod(
                "Parse",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);
        });

        return method?.Invoke(null, method.GetParameters().Length == 2
            ? [s, CultureInfo.InvariantCulture]
            : [s]);
    }
}