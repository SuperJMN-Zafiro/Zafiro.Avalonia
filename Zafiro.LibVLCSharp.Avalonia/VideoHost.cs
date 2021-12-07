using System.Diagnostics;
using Avalonia.Controls;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp;

public class VideoHost : Control, IMediaPlayer
{
    private VideoViewAdvanced view;

    public VideoViewAdvanced View
    {
        get => view;
        set
        {
            view = value;
            this.DataContextChanged += ViewOnDataContextChanged;
            VisualChildren.Add(view);
        }
    }

    private void ViewOnDataContextChanged(object? sender, EventArgs e)
    {
        view.DataContext = DataContext;

        if (DataContext is not null)
        {
            MessageBus.Current.SendMessage(new MediaPlayerCreated(this, DataContext));
        }
        else
        {
            Debug.WriteLine("DataContext is null in ViewViewAdvanced");
        }
    }

    public void Play()
    {
        view.Play();
    }

    public void SeekTo(TimeSpan beginning)
    {
        view.SeekTo(beginning);
    }

    public IObservable<TimeSpan> Position => view.PositionChanged;
    public void Stop()
    {
        view.Stop();
    }

    public void Pause()
    {
        view.Pause();
    }
}