using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Drag;

[Section("mdi-cursor-pointer", 8)]
public partial class DragViewModel : ReactiveObject
{
    [Reactive] private bool isEnabled;
    [Reactive] private double left;

    public DragViewModel()
    {
        Observable.Interval(TimeSpan.FromMilliseconds(24), AvaloniaScheduler.Instance).Subscribe(_ =>
        {
            if (IsEnabled)
            {
                Left += 1;
            }
        });

        this.WhenAnyValue(x => x.IsEnabled).Subscribe(b => Debug.WriteLine(b));
    }

    public ICommand Enable => ReactiveCommand.Create(() => IsEnabled = true);
    public ICommand Disable => ReactiveCommand.Create(() => IsEnabled = false);
}