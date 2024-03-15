using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.Controls;

public class Shell : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>> SectionsProperty = AvaloniaProperty.Register<Shell, IEnumerable<ISection>>(
        nameof(Sections));

    public IEnumerable<ISection> Sections
    {
        get => GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public static readonly StyledProperty<DataTemplate> IconTemplateProperty = AvaloniaProperty.Register<Shell, DataTemplate>(
        nameof(IconTemplate));

    public DataTemplate IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }
}