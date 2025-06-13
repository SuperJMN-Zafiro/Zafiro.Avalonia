using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class ActionBar : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>> SectionsProperty = AvaloniaProperty.Register<ActionBar, IEnumerable<ISection>>(
        nameof(Sections));

    public static readonly DirectProperty<ActionBar, IEnumerable<ISection>> VisibleSectionsProperty = AvaloniaProperty.RegisterDirect<ActionBar, IEnumerable<ISection>>(
        nameof(VisibleSections), o => o.VisibleSections, (o, v) => o.VisibleSections = v);

    public static readonly StyledProperty<int> ShowSectionsProperty = AvaloniaProperty.Register<ActionBar, int>(
        nameof(ShowSections), 4);

    public static readonly DirectProperty<ActionBar, IEnumerable<ISection>> OverflowSectionsProperty = AvaloniaProperty.RegisterDirect<ActionBar, IEnumerable<ISection>>(
        nameof(OverflowSections), o => o.OverflowSections, (o, v) => o.OverflowSections = v);

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<ActionBar, ISection>(
        nameof(SelectedSection));

    public static readonly StyledProperty<Dock> IconDockProperty = AvaloniaProperty.Register<ActionBar, Dock>(
        nameof(IconDock));

    private readonly CompositeDisposable disposable = new();

    private IEnumerable<ISection> overflowSections;


    private IEnumerable<ISection> visibleSections;

    public ActionBar()
    {
        this.WhenAnyValue(bottombar => bottombar.Sections, x => x.ShowSections)
            .Where((tuple, i) => tuple.Item1 != null)
            .Do(tuple => Update(tuple.Item2, tuple.Item1.ToList()))
            .Subscribe()
            .DisposeWith(disposable);
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

    public int ShowSections
    {
        get => GetValue(ShowSectionsProperty);
        set => SetValue(ShowSectionsProperty, value);
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

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposable.Dispose();
        base.OnUnloaded(e);
    }

    private void Update(int visibleCount, IList<ISection> items)
    {
        VisibleSections = items.Take(visibleCount);
        OverflowSections = items.Skip(visibleCount);
    }
}