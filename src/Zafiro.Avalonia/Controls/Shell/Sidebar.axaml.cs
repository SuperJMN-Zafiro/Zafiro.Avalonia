using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using DynamicData;
using DynamicData.Binding;
using Avalonia.Interactivity;
using ReactiveUI;
using Zafiro.Reactive;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class Sidebar : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ISection>> SectionsProperty = AvaloniaProperty.Register<Sidebar, IEnumerable<ISection>>(
        nameof(Sections));

    public static readonly DirectProperty<Sidebar, IEnumerable<ISection>> FilteredSectionsProperty = AvaloniaProperty.RegisterDirect<Sidebar, IEnumerable<ISection>>(
        nameof(FilteredSections), o => o.FilteredSections, (o, v) => o.FilteredSections = v);

    public static readonly StyledProperty<ISection> SelectedSectionProperty = AvaloniaProperty.Register<Sidebar, ISection>(
        nameof(SelectedSection), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> IconWidthProperty = AvaloniaProperty.Register<Sidebar, double>(
        nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = AvaloniaProperty.Register<Sidebar, double>(
        nameof(IconHeight));

    public static readonly StyledProperty<double> SectionSpacingProperty = AvaloniaProperty.Register<Sidebar, double>(
        nameof(SectionSpacing));

    public static readonly StyledProperty<Thickness> SectionNameMarginProperty = AvaloniaProperty.Register<Sidebar, Thickness>(
        nameof(SectionNameMargin));

    public static readonly StyledProperty<Thickness> IconMarginProperty = AvaloniaProperty.Register<Sidebar, Thickness>(
        nameof(IconMargin));

    public static readonly StyledProperty<IDataTemplate> IconTemplateProperty = AvaloniaProperty.Register<Sidebar, IDataTemplate>(
        nameof(IconTemplate));

    private readonly CompositeDisposable disposables = new();
    private IEnumerable<ISection> filteredSections = Array.Empty<ISection>();

    public Sidebar()
    {
        var changeSet = this.WhenAnyValue(x => x.Sections)
            .Where(sections => sections is not null)
            .Select(sections => sections!.OfType<INamedSection>().ToObservableChangeSetIfPossible(s => s.Name))
            .Switch();

        BuildFiltered(changeSet)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(list => FilteredSections = list)
            .DisposeWith(disposables);
    }

    private static IObservable<IEnumerable<ISection>> BuildFiltered(IObservable<IChangeSet<INamedSection, string>> changes)
    {
        return changes
            .FilterOnObservable(s => s.IsVisible)
            .Transform(s => new SectionEntry(s))
            .DisposeMany()
            .AutoRefresh(x => x.Order)
            .Sort(SortExpressionComparer<SectionEntry>
                .Ascending(x => x.Order)
                .ThenByDescending(x => x.Section.IsPrimary)
                .ThenBy(x => x.Section.FriendlyName))
            .Transform(x => (ISection)x.Section)
            .ToCollection()
            .Select(list => (IEnumerable<ISection>)list);
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

    public IEnumerable<ISection> FilteredSections
    {
        get => filteredSections;
        set => SetAndRaise(FilteredSectionsProperty, ref filteredSections, value);
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

    public double SectionSpacing
    {
        get => GetValue(SectionSpacingProperty);
        set => SetValue(SectionSpacingProperty, value);
    }

    public Thickness SectionNameMargin
    {
        get => GetValue(SectionNameMarginProperty);
        set => SetValue(SectionNameMarginProperty, value);
    }

    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    public IDataTemplate IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }
}
