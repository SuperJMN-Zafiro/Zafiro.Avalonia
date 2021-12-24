using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Metadata;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp;

public class VideoHost : Control
{
    private VideoView view;

    [Content]
    public VideoView View
    {
        get => view;
        set
        {
            view = value;
            this.DataContextChanged += ViewOnDataContextChanged;
            LogicalChildren.Add(view);
            VisualChildren.Add(view);
        }
    }

    private void ViewOnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not null)
        {
            MessageBus.Current.SendMessage(new MediaPlayerCreated(view, DataContext));
        }
        else
        {
            Debug.WriteLine("DataContext is null in ViewViewAdvanced");
        }
    }
}