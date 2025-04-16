using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia.Controls;

public class Loading : ContentControl
{
    public static readonly StyledProperty<IControlTemplate> LoadingTemplateProperty = AvaloniaProperty.Register<Loading, IControlTemplate>(
        nameof(LoadingTemplate));

    public IControlTemplate LoadingTemplate
    {
        get => GetValue(LoadingTemplateProperty);
        set => SetValue(LoadingTemplateProperty, value);
    }

    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<Loading, bool>(
        nameof(IsLoading));

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly StyledProperty<string> LoadingTextProperty = AvaloniaProperty.Register<Loading, string>(
        nameof(LoadingText), "Loading...");

    public string LoadingText
    {
        get => GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public static readonly StyledProperty<double> ProgressIndicatorSizeProperty = AvaloniaProperty.Register<Loading, double>(
        nameof(ProgressIndicatorSize), 64);

    public double ProgressIndicatorSize
    {
        get => GetValue(ProgressIndicatorSizeProperty);
        set => SetValue(ProgressIndicatorSizeProperty, value);
    }
}