using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.Controls.Diagrams;

public static class HaveLocationExtensions
{
    public static Point Location(this IHaveLocation location)
    {
        return new Point(location.Left, location.Top);
    }

    public static IObservable<Rect> BoundsChanged<T>(this IEdge<T> location) where T : IHaveLocation
    {
        return location.To.LocationChanged().CombineLatest(location.From.LocationChanged(), (a, b) =>
        {
            var topLeft = new Point(
                Math.Min(a.X, b.X), 
                Math.Min(a.Y, b.Y)
            );
    
            var bottomRight = new Point(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y)
            );

            return new Rect(topLeft, bottomRight);
        });
    }

    public static IObservable<Point> LocationChanged(this IHaveLocation location)
    {
        return location.WhenAnyValue(x => x.Left, x => x.Top, (x, y) => new Point(x, y));
    }
}