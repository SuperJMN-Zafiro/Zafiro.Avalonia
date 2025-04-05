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
}