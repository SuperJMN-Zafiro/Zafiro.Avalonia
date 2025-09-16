using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls;

public class SectionStrip : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>?> SectionsProperty = AvaloniaProperty.Register<SectionStrip, IEnumerable<ISection>?>(
        nameof(Sections));

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<SectionStrip, ISection>(nameof(SelectedSection), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<SectionStrip, Orientation>(
        nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<double> MaxItemWidthProperty = AvaloniaProperty.Register<SectionStrip, double>(
        nameof(MaxItemWidth));

    public static readonly StyledProperty<double> MinItemWidthProperty = AvaloniaProperty.Register<SectionStrip, double>(
        nameof(MinItemWidth));

    public static readonly StyledProperty<double> ItemSpacingProperty = AvaloniaProperty.Register<SectionStrip, double>(
        nameof(ItemSpacing));

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

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double MaxItemWidth
    {
        get => GetValue(MaxItemWidthProperty);
        set => SetValue(MaxItemWidthProperty, value);
    }

    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }
}