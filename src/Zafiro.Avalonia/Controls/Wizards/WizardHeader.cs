using Avalonia.Controls.Primitives;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls.Wizards;

public class WizardHeader : TemplatedControl
{
    public static readonly StyledProperty<Maybe<string>> TitleProperty = AvaloniaProperty.Register<WizardHeader, Maybe<string>>(
        nameof(Title));

    public Maybe<string> Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<int> CurrentPageIndexProperty = AvaloniaProperty.Register<WizardHeader, int>(
        nameof(CurrentPageIndex));

    public int CurrentPageIndex
    {
        get => GetValue(CurrentPageIndexProperty);
        set => SetValue(CurrentPageIndexProperty, value);
    }

    public static readonly StyledProperty<int> TotalPagesProperty = AvaloniaProperty.Register<WizardHeader, int>(
        nameof(TotalPages));

    public int TotalPages
    {
        get => GetValue(TotalPagesProperty);
        set => SetValue(TotalPagesProperty, value);
    }

    public static readonly StyledProperty<IEnhancedCommand> BackProperty = AvaloniaProperty.Register<WizardHeader, IEnhancedCommand>(
        nameof(Back));

    public IEnhancedCommand Back
    {
        get => GetValue(BackProperty);
        set => SetValue(BackProperty, value);
    }
}