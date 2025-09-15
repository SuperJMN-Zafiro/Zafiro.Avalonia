using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Reactive;
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


    private IEnumerable<ISection> visibleSections = Array.Empty<ISection>();

    private IDisposable? sectionsSubscription;

    public ActionBar()
    {
        var sectionsChanges = this.WhenAnyValue(x => x.Sections)
            .Where(sections => sections is not null)
            .Select(sections => sections!.OfType<INamedSection>().ToObservableChangeSetIfPossible(s => s.Name))
            .Switch();

        var columnsChanges = this.WhenAnyValue(x => x.Columns).StartWith(Columns);

        sectionsSubscription = BuildPipeline(sectionsChanges, columnsChanges)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(result =>
            {
                VisibleSections = result.visible;
                OverflowSections = result.overflow;
            });

        sectionsSubscription.DisposeWith(disposable);
    }

    private static IObservable<(IEnumerable<ISection> visible, IEnumerable<ISection> overflow)> BuildPipeline(IObservable<IChangeSet<INamedSection, string>> changes, IObservable<int> columns)
    {
        var sorted = changes
            .FilterOnObservable(s => s.IsVisible)
            .Transform(s => new SectionEntry(s))
            .DisposeMany()
            .AutoRefresh(x => x.Order)
            .Sort(SortExpressionComparer<SectionEntry>
                .Ascending(x => x.Order)
                .ThenByDescending(x => x.Section.IsPrimary)
                .ThenBy(x => x.Section.FriendlyName))
            .Transform(x => (ISection)x.Section)
            .ToCollection();

        return sorted.CombineLatest(columns, (list, visibleCount) =>
        {
            var visible = list.Take(visibleCount).ToList();
            var overflow = list.Skip(visibleCount).ToList();
            return ((IEnumerable<ISection>)visible, (IEnumerable<ISection>)overflow);
        });
    }

    private sealed class SectionEntry : INotifyPropertyChanged, IDisposable
    {
        private int order;
        private readonly IDisposable subscription;
        public INamedSection Section { get; }

        public SectionEntry(INamedSection section)
        {
            Section = section;
            subscription = section.SortOrder.Subscribe(value => Order = value);
        }

        public int Order
        {
            get => order;
            private set
            {
                if (order == value) return;
                order = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
