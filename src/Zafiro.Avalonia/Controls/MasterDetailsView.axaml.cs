using System.Collections;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using JetBrains.Annotations;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls;

[PublicAPI]
public class MasterDetailsView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty = AvaloniaProperty.Register<MasterDetailsView, IEnumerable>(
        nameof(ItemsSource));

    public static readonly StyledProperty<object> SelectedItemProperty = AvaloniaProperty.Register<MasterDetailsView, object>(
        nameof(SelectedItem));

    public static readonly DirectProperty<MasterDetailsView, ICommand> GoToDetailsProperty = AvaloniaProperty.RegisterDirect<MasterDetailsView, ICommand>(
        nameof(GoToDetails), o => o.GoToDetails, (o, v) => o.GoToDetails = v);

    public static readonly DirectProperty<MasterDetailsView, ICommand> BackCommandProperty = AvaloniaProperty.RegisterDirect<MasterDetailsView, ICommand>(
        nameof(BackCommand), o => o.BackCommand, (o, v) => o.BackCommand = v);

    public static readonly StyledProperty<bool> IsBackButtonEnabledProperty = AvaloniaProperty.Register<MasterDetailsView, bool>(nameof(IsBackButtonDisplayed), true);

    public static readonly StyledProperty<DataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<MasterDetailsView, DataTemplate>(
        nameof(ItemTemplate));

    public static readonly StyledProperty<DataTemplate> DetailsTemplateProperty = AvaloniaProperty.Register<MasterDetailsView, DataTemplate>(
        nameof(DetailsTemplate));

    public static readonly DirectProperty<MasterDetailsView, bool> AreDetailsShownProperty = AvaloniaProperty.RegisterDirect<MasterDetailsView, bool>(
        nameof(AreDetailsShown), o => o.AreDetailsShown, (o, v) => o.AreDetailsShown = v);

    public static readonly StyledProperty<double> CompactWidthProperty = AvaloniaProperty.Register<MasterDetailsView, double>(nameof(CompactWidth), 400);

    public static readonly StyledProperty<object> ItemsProperty = AvaloniaProperty.Register<MasterDetailsView, object>(nameof(Items));

    public static readonly StyledProperty<double> MasterPaneWidthProperty = AvaloniaProperty.Register<MasterDetailsView, double>(nameof(MasterPaneWidth), 200);

    public double MasterPaneWidth
    {
        get => GetValue(MasterPaneWidthProperty);
        set => SetValue(MasterPaneWidthProperty, value);
    }

    private bool areDetailsShown;

    private ICommand backCommand = null!;

    private ICommand goToDetails = null!;

    public MasterDetailsView()
    {
        this.WhenAnyValue(x => x.SelectedItem)
            .WhereNotNull()
            .Do(_ => AreDetailsShown = true)
            .Subscribe();

        BackCommand = ReactiveCommand.Create(() => AreDetailsShown = false);
        GoToDetails = ReactiveCommand.Create(() => AreDetailsShown = true);
    }

    public double CompactWidth
    {
        get => GetValue(CompactWidthProperty);
        set => SetValue(CompactWidthProperty, value);
    }

    [Content]
    public object Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public DataTemplate DetailsTemplate
    {
        get => GetValue(DetailsTemplateProperty);
        set => SetValue(DetailsTemplateProperty, value);
    }

    public bool IsBackButtonDisplayed
    {
        get => GetValue(IsBackButtonEnabledProperty);
        set => SetValue(IsBackButtonEnabledProperty, value);
    }

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool AreDetailsShown
    {
        get => areDetailsShown;
        private set => SetAndRaise(AreDetailsShownProperty, ref areDetailsShown, value);
    }

    public ICommand GoToDetails
    {
        get => goToDetails;
        private set => SetAndRaise(GoToDetailsProperty, ref goToDetails, value);
    }

    public ICommand BackCommand
    {
        get => backCommand;
        private set => SetAndRaise(BackCommandProperty, ref backCommand, value);
    }

    public DataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
}