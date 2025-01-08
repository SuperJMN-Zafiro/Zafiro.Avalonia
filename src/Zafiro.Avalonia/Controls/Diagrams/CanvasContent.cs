using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class CanvasContent : ContentControl
{
    private readonly IDisposable positionSubscription;
    
    public CanvasContent(IObservable<Point> positions)
    {
        positionSubscription = positions.Do(point =>
        {
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
        }).Subscribe();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        positionSubscription.Dispose();
        base.OnUnloaded(e);
    }
}