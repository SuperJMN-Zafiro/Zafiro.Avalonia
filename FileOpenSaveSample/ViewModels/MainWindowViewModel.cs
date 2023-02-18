using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro.Core.Mixins;
using Zafiro.FileSystem;

namespace FileOpenSaveSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {
            var currentApplicationLifetime = Application.Current.ApplicationLifetime;
            var mainWindow = (currentApplicationLifetime as ClassicDesktopStyleApplicationLifetime).MainWindow;
            var picker = new AvaloniaDesktopFilePicker(mainWindow, new ZafiroFileSystem(new FileSystem(), Maybe<ILogger>.None));
            Open = ReactiveCommand.CreateFromObservable(() => picker.PickSingle(("Pepito", new[] { "jpg" })));
            OpenMultiple = ReactiveCommand.CreateFromObservable(() => picker.PickMultiple());
            Open.WhereSuccess().Subscribe(x => { });
            OpenMultiple.Select(x => x.WhereSuccess()).Subscribe(b => { });
        }

        public ReactiveCommand<Unit, IEnumerable<Result<IZafiroFile>>> OpenMultiple { get; }

        public ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }
    }
}
