using System.Reactive.Disposables;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Controls.Shell;

public class ShellView : TemplatedControl
{
    public static readonly StyledProperty<IShell> ShellProperty = AvaloniaProperty.Register<ShellView, IShell>(
        nameof(Shell));

    public static readonly StyledProperty<double> OpenPaneLengthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(OpenPaneLength));

    public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty = AvaloniaProperty.Register<ShellView, IDataTemplate>(
        nameof(HeaderContentTemplate));

    public static readonly StyledProperty<double> MenuButtonSizeProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(MenuButtonSize));

    public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<ShellView, bool>(
        nameof(IsPaneOpen));

    public static readonly StyledProperty<IPageTransition> PageTransitionProperty = AvaloniaProperty.Register<ShellView, IPageTransition>(
        nameof(PageTransition));

    public static readonly DirectProperty<ShellView, bool> useDesktopLayoutProperty = AvaloniaProperty.RegisterDirect<ShellView, bool>(
        nameof(UseDesktopLayout), o => o.UseDesktopLayout, (o, v) => o.UseDesktopLayout = v);

    public static readonly StyledProperty<bool> ForceDesktopLayoutProperty = AvaloniaProperty.Register<ShellView, bool>(
        nameof(ForceDesktopLayout));

    public static readonly StyledProperty<int> MobileColumnsProperty = AvaloniaProperty.Register<ShellView, int>(
        nameof(MobileColumns));

    public static readonly StyledProperty<IDataTemplate> IconTemplateProperty = AvaloniaProperty.Register<ShellView, IDataTemplate>(
        nameof(IconTemplate));

    public static readonly StyledProperty<IBrush> PaneBackgroundProperty = AvaloniaProperty.Register<ShellView, IBrush>(
        nameof(PaneBackground));

    public static readonly StyledProperty<ControlTheme> ToggleButtonThemeProperty = AvaloniaProperty.Register<ShellView, ControlTheme>(
        nameof(ToggleButtonTheme));

    public static readonly StyledProperty<Thickness> SectionItemPaddingProperty = AvaloniaProperty.Register<ShellView, Thickness>(
        nameof(SectionItemPadding));

    // Separate spacings for icon/label in each orientation
    public static readonly StyledProperty<double> SectionHorizontalIconLabelSpacingProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(SectionHorizontalIconLabelSpacing));

    public static readonly StyledProperty<double> SectionVerticalIconLabelSpacingProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(SectionVerticalIconLabelSpacing));

    public static readonly StyledProperty<double> SectionIconSizeProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(SectionIconSize));

    public static readonly StyledProperty<double> SectionMaxItemWidthProperty = AvaloniaProperty.Register<ShellView, double>(
        nameof(SectionMaxItemWidth));

    public static readonly StyledProperty<Thickness> IconMarginProperty = AvaloniaProperty.Register<ShellView, Thickness>(
        nameof(IconMargin));

    private readonly CompositeDisposable disposable = new();

    private bool useDesktopLayout;

    public ShellView()
    {
        this.WhenAnyValue(view => view.ForceDesktopLayout).Select(forceDesktop => forceDesktop || IsDesktop)
            .BindTo(this, x => x.UseDesktopLayout)
            .DisposeWith(disposable);
    }

    public IDataTemplate IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public IShell Shell
    {
        get => GetValue(ShellProperty);
        set => SetValue(ShellProperty, value);
    }

    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    public IDataTemplate HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }

    public double MenuButtonSize
    {
        get => GetValue(MenuButtonSizeProperty);
        set => SetValue(MenuButtonSizeProperty, value);
    }

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public IPageTransition PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    public bool UseDesktopLayout
    {
        get => useDesktopLayout;
        set => SetAndRaise(useDesktopLayoutProperty, ref useDesktopLayout, value);
    }

    public bool ForceDesktopLayout
    {
        get => GetValue(ForceDesktopLayoutProperty);
        set => SetValue(ForceDesktopLayoutProperty, value);
    }

    private static bool IsDesktop => OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();

    public int MobileColumns
    {
        get => GetValue(MobileColumnsProperty);
        set => SetValue(MobileColumnsProperty, value);
    }

    public IBrush PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }

    public ControlTheme ToggleButtonTheme
    {
        get => GetValue(ToggleButtonThemeProperty);
        set => SetValue(ToggleButtonThemeProperty, value);
    }

    public Thickness SectionItemPadding
    {
        get => GetValue(SectionItemPaddingProperty);
        set => SetValue(SectionItemPaddingProperty, value);
    }

    public double SectionHorizontalIconLabelSpacing
    {
        get => GetValue(SectionHorizontalIconLabelSpacingProperty);
        set => SetValue(SectionHorizontalIconLabelSpacingProperty, value);
    }

    public double SectionVerticalIconLabelSpacing
    {
        get => GetValue(SectionVerticalIconLabelSpacingProperty);
        set => SetValue(SectionVerticalIconLabelSpacingProperty, value);
    }

    public double SectionIconSize
    {
        get => GetValue(SectionIconSizeProperty);
        set => SetValue(SectionIconSizeProperty, value);
    }

    public double SectionMaxItemWidth
    {
        get => GetValue(SectionMaxItemWidthProperty);
        set => SetValue(SectionMaxItemWidthProperty, value);
    }

    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposable.Dispose();
        base.OnUnloaded(e);
    }
}