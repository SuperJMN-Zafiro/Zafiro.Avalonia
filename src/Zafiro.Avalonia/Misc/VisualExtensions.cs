using Avalonia.VisualTree;

namespace Zafiro.Avalonia.ViewLocators;

public static class VisualExtensions
{
    public static Vector GetEffectiveScale(this Visual visual)
    {
        var transform = visual.GetTransformedBounds();
        if (transform == null)
        {
            return new Vector(1, 1);
        }

        var matrix = transform.Value.Transform;
        var scaleX = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
        var scaleY = Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22);

        // Avoid divisions by zero
        scaleX = scaleX == 0 ? 1 : scaleX;
        scaleY = scaleY == 0 ? 1 : scaleY;

        return new Vector(scaleX, scaleY);
    }

    public static IObservable<Vector> EffectiveScale(this Visual visual, TimeSpan timeSpan)
    {
        return Observable.Interval(timeSpan, AvaloniaScheduler.Instance)
            .Select(_ => visual.GetEffectiveScale())
            .DistinctUntilChanged();
    }
}