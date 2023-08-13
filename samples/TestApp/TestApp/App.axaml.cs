using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TestApp.ViewModels;
using TestApp.Views;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Storage;

namespace TestApp;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), view => new MainViewModel(DialogService.Create(Current!.ApplicationLifetime!, new Dictionary<Type, Type>()), new AvaloniaFilePicker(TopLevel.GetTopLevel(view)!.StorageProvider)), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}