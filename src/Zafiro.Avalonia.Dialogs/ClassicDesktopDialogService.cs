﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Zafiro.Avalonia.Dialogs;

public class ClassicDesktopDialogService : DialogService
{
    public ClassicDesktopDialogService(IReadOnlyDictionary<Type, Type> modelToViewDictionary) : base(modelToViewDictionary)
    {
    }

    public override Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options)
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var window = new Window
        {
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            MaxWidth = 640,
        };

        var wrapper = new WindowWrapper(window);
        
        window.Content = new DialogView { DataContext = new DialogViewModel(GetFinalContent(viewModel), title, CreateOptions(viewModel, wrapper, options).ToArray()) };
        
        return window.ShowDialog(MainWindow);
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}