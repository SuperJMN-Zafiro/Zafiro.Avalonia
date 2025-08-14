using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class CardGrid : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<CardGrid, IEnumerable>(nameof(ItemsSource));

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<CardGrid, IDataTemplate>(
        nameof(ItemTemplate));

    public static readonly StyledProperty<double> RowSpacingProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(RowSpacing));

    public static readonly StyledProperty<double> ColumnSpacingProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(ColumnSpacing));

    public static readonly StyledProperty<ControlTheme> ItemContainerThemeProperty = AvaloniaProperty.Register<CardGrid, ControlTheme>(
        nameof(ItemContainerTheme));

    public static readonly StyledProperty<double> MinColumnWidthProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(MinColumnWidth), 200d);

    public static readonly StyledProperty<double> MaxColumnWidthProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(MaxColumnWidth), double.PositiveInfinity);

    public double MaxColumnWidth
    {
        get => GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public ControlTheme ItemContainerTheme
    {
        get => GetValue(ItemContainerThemeProperty);
        set => SetValue(ItemContainerThemeProperty, value);
    }

    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }
}