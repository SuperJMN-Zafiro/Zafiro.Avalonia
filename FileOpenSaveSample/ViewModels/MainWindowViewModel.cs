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
using Zafiro.Avalonia;
using Zafiro.Core.Mixins;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace FileOpenSaveSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {
            var currentApplicationLifetime = Application.Current.ApplicationLifetime;
            var mainWindow = (currentApplicationLifetime as ClassicDesktopStyleApplicationLifetime).MainWindow;
            var zafiroFileSystem = new ZafiroFileSystem(new FileSystem(), Maybe<ILogger>.None);

            var fileOpenPicker = new DesktopOpenFilePicker(mainWindow, zafiroFileSystem);
            var filter = new FileTypeFilter("Images", "jpg");
            Open = ReactiveCommand.CreateFromObservable(() => fileOpenPicker.PickSingle(filter));
            OpenMultiple = ReactiveCommand.CreateFromObservable(() => fileOpenPicker.PickMultiple());
            Open.WhereSuccess().Subscribe(x => { });
            OpenMultiple.Select(x => x.WhereSuccess()).Subscribe(b => { });
            Save = ReactiveCommand.CreateFromObservable(() => new DesktopSaveFilePicker(mainWindow, zafiroFileSystem).Pick(filter));
            Save.Subscribe(selected => { });
        }

        public ReactiveCommand<Unit, Result<IZafiroFile>> Save { get; }

        public ReactiveCommand<Unit, IEnumerable<Result<IZafiroFile>>> OpenMultiple { get; }

        public ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }
    }
}
