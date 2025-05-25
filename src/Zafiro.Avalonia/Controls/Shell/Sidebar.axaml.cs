using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class Sidebar : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>> SectionsProperty = AvaloniaProperty.Register<Sidebar, IEnumerable<ISection>>(
        nameof(Sections));

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<Sidebar, ISection>(
        nameof(SelectedSection), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> IconWidthProperty = AvaloniaProperty.Register<Sidebar, double>(
        nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = AvaloniaProperty.Register<Sidebar, double>(
        nameof(IconHeight));

    public IEnumerable<ISection> Sections
    {
        get => GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public ISection SelectedSection
    {
        get => GetValue(SelectedSectionProperty);
        set => SetValue(SelectedSectionProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
}