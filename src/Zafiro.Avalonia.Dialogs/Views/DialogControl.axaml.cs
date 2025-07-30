using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.Views;

public class DialogControl : ContentControl
{
    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<DialogControl, string?>(
        nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<DialogControl, object?>(
        nameof(Header));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<IOption>?> OptionsProperty = AvaloniaProperty.Register<DialogControl, IEnumerable<IOption>?>(
        nameof(Options),
        defaultValue: []);

    public IEnumerable<IOption>? Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    private static readonly DirectProperty<DialogControl, IEnumerable<IOption>> PrimaryOptionsProperty =
        AvaloniaProperty.RegisterDirect<DialogControl, IEnumerable<IOption>>(
            nameof(PrimaryOptions),
            o => o.PrimaryOptions);

    private IEnumerable<IOption> primaryOptions = [];
    public IEnumerable<IOption> PrimaryOptions
    {
        get => primaryOptions;
        private set => SetAndRaise(PrimaryOptionsProperty, ref primaryOptions, value);
    }

    private static readonly DirectProperty<DialogControl, IEnumerable<IOption>> CancelOptionsProperty =
        AvaloniaProperty.RegisterDirect<DialogControl, IEnumerable<IOption>>(
            nameof(CancelOptions),
            o => o.CancelOptions);

    private IEnumerable<IOption> cancelOptions = [];
    public IEnumerable<IOption> CancelOptions
    {
        get => cancelOptions;
        private set => SetAndRaise(CancelOptionsProperty, ref cancelOptions, value);
    }

    private static readonly DirectProperty<DialogControl, IEnumerable<IOption>> DestructiveOptionsProperty =
        AvaloniaProperty.RegisterDirect<DialogControl, IEnumerable<IOption>>(
            nameof(DestructiveOptions),
            o => o.DestructiveOptions);

    private IEnumerable<IOption> destructiveOptions = [];
    public IEnumerable<IOption> DestructiveOptions
    {
        get => destructiveOptions;
        private set => SetAndRaise(DestructiveOptionsProperty, ref destructiveOptions, value);
    }

    private static readonly DirectProperty<DialogControl, IEnumerable<IOption>> SecondaryOptionsProperty =
        AvaloniaProperty.RegisterDirect<DialogControl, IEnumerable<IOption>>(
            nameof(SecondaryOptions),
            o => o.SecondaryOptions);

    private IEnumerable<IOption> secondaryOptions = Enumerable.Empty<IOption>();
    public IEnumerable<IOption> SecondaryOptions
    {
        get => secondaryOptions;
        private set => SetAndRaise(SecondaryOptionsProperty, ref secondaryOptions, value);
    }

    private static readonly DirectProperty<DialogControl, IEnumerable<IOption>> InfoOptionsProperty =
        AvaloniaProperty.RegisterDirect<DialogControl, IEnumerable<IOption>>(
            nameof(InfoOptions),
            o => o.InfoOptions);

    private IEnumerable<IOption> infoOptions = [];
    public IEnumerable<IOption> InfoOptions
    {
        get => infoOptions;
        private set => SetAndRaise(InfoOptionsProperty, ref infoOptions, value);
    }

    public DialogControl()
    {
        // Suscribe al cambio de Options
        this.GetObservable(OptionsProperty).Subscribe(options =>
        {
            UpdateDerivedProperties(options);
        });
    }

    private void UpdateDerivedProperties(IEnumerable<IOption>? options)
    {
        var safeOptions = options ?? [];

        var opts = safeOptions.ToList();
        
        PrimaryOptions = opts.Where(o => o.Role == OptionRole.Primary).ToList();
        CancelOptions = opts.Where(o => o.Role == OptionRole.Cancel).ToList();
        DestructiveOptions = opts.Where(o => o.Role == OptionRole.Destructive).ToList();
        SecondaryOptions = opts.Where(o => o.Role == OptionRole.Secondary).ToList();
        InfoOptions = opts.Where(o => o.Role == OptionRole.Info).ToList();
    }
}