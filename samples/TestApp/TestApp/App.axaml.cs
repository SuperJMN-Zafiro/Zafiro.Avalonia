using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CSharpFunctionalExtensions;
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
        this.Connect(() => new MainView(), view => new MainViewModel(DialogService.Create(Current!.ApplicationLifetime!, new Dictionary<Type, Type>(), Maybe<Action<ConfigureWindowContext>>.From(context => context.ToConfigure.SizeToContent = SizeToContent.WidthAndHeight)), new AvaloniaFilePicker(TopLevel.GetTopLevel(view)!.StorageProvider)), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }

    private void AQUI_TOY_OnDataContextChanged(object? sender, EventArgs e)
    {
        
    }
}