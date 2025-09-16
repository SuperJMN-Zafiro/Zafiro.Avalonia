using Avalonia.Controls.Primitives;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls;

public class ActionBar2 : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>?> SectionsProperty = AvaloniaProperty.Register<ActionBar2, IEnumerable<ISection>?>(
        nameof(Sections));

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<ActionBar2, ISection>(
        nameof(SelectedSection));

    public IEnumerable<ISection>? Sections
    {
        get => GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public ISection SelectedSection
    {
        get => GetValue(SelectedSectionProperty);
        set => SetValue(SelectedSectionProperty, value);
    }
}