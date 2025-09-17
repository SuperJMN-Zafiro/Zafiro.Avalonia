using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class ActionBar : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>> SectionsProperty = AvaloniaProperty.Register<ActionBar, IEnumerable<ISection>>(
        nameof(Sections));

    public static readonly DirectProperty<ActionBar, IEnumerable<ISection>> VisibleSectionsProperty = AvaloniaProperty.RegisterDirect<ActionBar, IEnumerable<ISection>>(
        nameof(VisibleSections), o => o.VisibleSections, (o, v) => o.VisibleSections = v);

    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<ActionBar, int>(
        nameof(Columns), 4);

    public static readonly DirectProperty<ActionBar, IEnumerable<ISection>> OverflowSectionsProperty = AvaloniaProperty.RegisterDirect<ActionBar, IEnumerable<ISection>>(
        nameof(OverflowSections), o => o.OverflowSections, (o, v) => o.OverflowSections = v);

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<ActionBar, ISection>(
        nameof(SelectedSection), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Dock> IconDockProperty = AvaloniaProperty.Register<ActionBar, Dock>(
        nameof(IconDock));

    public static readonly StyledProperty<IDataTemplate> IconTemplateProperty = AvaloniaProperty.Register<ActionBar, IDataTemplate>(
        nameof(IconTemplate));

    private readonly CompositeDisposable disposable = new();

    private IEnumerable<ISection> overflowSections = Array.Empty<ISection>();

    private IDisposable? sectionsSubscription;


    private IEnumerable<ISection> visibleSections = Array.Empty<ISection>();

    public ActionBar()
    {
        sectionsSubscription.DisposeWith(disposable);
    }


    public IEnumerable<ISection> Sections
    {
        get => GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public IEnumerable<ISection> VisibleSections
    {
        get => visibleSections;
        set => SetAndRaise(VisibleSectionsProperty, ref visibleSections, value);
    }

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public IEnumerable<ISection> OverflowSections
    {
        get => overflowSections;
        set => SetAndRaise(OverflowSectionsProperty, ref overflowSections, value);
    }

    public ISection SelectedSection
    {
        get => GetValue(SelectedSectionProperty);
        set => SetValue(SelectedSectionProperty, value);
    }

    public Dock IconDock
    {
        get => GetValue(IconDockProperty);
        set => SetValue(IconDockProperty, value);
    }

    public IDataTemplate IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposable.Dispose();
        base.OnUnloaded(e);
    }
}