using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.SourceGenerators;
using Zafiro.Reactive;
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

    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<SectionStrip, double>(
        nameof(IconSize), defaultValue: 38d);

    public static readonly StyledProperty<double> IconLabelSpacingProperty = AvaloniaProperty.Register<SectionStrip, double>(
        nameof(IconLabelSpacing));

    public static readonly StyledProperty<Thickness> ItemPaddingProperty = AvaloniaProperty.Register<SectionStrip, Thickness>(
        nameof(ItemPadding));

    public static readonly DirectProperty<SectionStrip, IEnumerable<ISection>> FilteredSectionsProperty = AvaloniaProperty.RegisterDirect<SectionStrip, IEnumerable<ISection>>(
        nameof(FilteredSections), o => o.FilteredSections, (o, v) => o.FilteredSections = v);

    private readonly CompositeDisposable disposable = new();

    private IEnumerable<ISection> filteredSections;

    public SectionStrip()
    {
        var sectionChanges = this.WhenAnyValue(strip => strip.Sections)
            .WhereNotNull()
            .Select(sections => sections.OfType<INamedSection>().ToObservableChangeSetIfPossible(section => section.Name))
            .Switch();

        var sectionSorter = new SectionSorter(sectionChanges)
            .DisposeWith(disposable);

        FilteredSections = sectionSorter.Sections;
    }

    public IEnumerable<ISection> FilteredSections
    {
        get => filteredSections;
        set => SetAndRaise(FilteredSectionsProperty, ref filteredSections, value);
    }

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

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public double IconLabelSpacing
    {
        get => GetValue(IconLabelSpacingProperty);
        set => SetValue(IconLabelSpacingProperty, value);
    }

    public Thickness ItemPadding
    {
        get => GetValue(ItemPaddingProperty);
        set => SetValue(ItemPaddingProperty, value);
    }
}

public partial class SectionWrapper(ISection section) : ReactiveObject, ISection
{
    [Reactive] private int order;
    public ISection Section { get; } = section;
    public bool IsPrimary { get; init; } = section.IsPrimary;
    public IObservable<bool> IsVisible { get; init; } = section.IsVisible;
    public IObservable<int> SortOrder { get; init; } = section.SortOrder;
}

public sealed class SectionSorter : IDisposable
{
    private readonly CompositeDisposable disposable = new();

    public SectionSorter(IObservable<IChangeSet<INamedSection, string>> sectionChanges)
    {
        var sortingOrderChanged = sectionChanges.MergeMany(section => section.SortOrder)
            .Throttle(TimeSpan.FromMilliseconds(250), AvaloniaScheduler.Instance)
            .Select(_ => Unit.Default);

        sectionChanges
            .Transform(section => new SectionWrapper(section))
            .FilterOnObservable(section => section.IsVisible)
            .Sort(comparer: SortExpressionComparer<SectionWrapper>.Descending(wrapper => wrapper.Order), resorter: sortingOrderChanged)
            .Transform(wrapper => wrapper.Section)
            .Bind(out var filtered)
            .Subscribe()
            .DisposeWith(disposable);

        Sections = filtered;
    }

    public ReadOnlyObservableCollection<ISection> Sections { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}