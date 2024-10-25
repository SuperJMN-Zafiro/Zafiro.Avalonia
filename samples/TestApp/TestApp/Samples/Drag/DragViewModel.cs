using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TestApp.Samples.Drag;

public partial class DragViewModel : ReactiveObject
{
    public DragViewModel()
    {
        Observable.Interval(TimeSpan.FromMilliseconds(24), AvaloniaScheduler.Instance).Subscribe(_ =>
        {
            if (IsEnabled)
            {
                Left+=1;
            }
        });

        this.WhenAnyValue(x => x.IsEnabled).Subscribe(b => Debug.WriteLine(b));
    }

    [Reactive] private bool isEnabled;
    [Reactive] private double left;
    public ICommand Enable => ReactiveCommand.Create(() => IsEnabled = true);
    public ICommand Disable => ReactiveCommand.Create(() => IsEnabled = false);
}