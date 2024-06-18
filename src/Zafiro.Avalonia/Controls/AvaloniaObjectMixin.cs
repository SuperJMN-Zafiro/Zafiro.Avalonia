namespace Zafiro.Avalonia.Controls;

public static class AvaloniaObjectMixin
{
    public static IDisposable ToProperty<T>(this IObservable<T> observable, AvaloniaObject avaloniaObject, AvaloniaProperty<T> property) => avaloniaObject.Bind(property, observable);
}