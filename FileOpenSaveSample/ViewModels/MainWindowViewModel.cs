using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia;
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
            var fileOpenPicker = new OpenFilePicker(mainWindow);
            var filter = new FileTypeFilter("Images", "*.jpg");
            Open = ReactiveCommand.CreateFromObservable(() => fileOpenPicker.PickSingle(filter));
            OpenMultiple = ReactiveCommand.CreateFromObservable(() => fileOpenPicker.PickMultiple());
            Open.Subscribe(x => { });
            OpenMultiple.Subscribe(b => { });
            Save = ReactiveCommand.CreateFromObservable(() => new SaveFilePicker(mainWindow).Pick(filter));
            Save.Subscribe(selected => { });
        }

        public ReactiveCommand<Unit, Maybe<IStorable?>> Save { get; }

        public ReactiveCommand<Unit, IEnumerable<IStorable>> OpenMultiple { get; }

        public ReactiveCommand<Unit, Maybe<IStorable>> Open { get; }
    }
}
