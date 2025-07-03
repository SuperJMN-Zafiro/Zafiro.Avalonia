using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class CardGrid : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<CardGrid, IEnumerable>(nameof(ItemsSource));

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<CardGrid, IDataTemplate>(
        nameof(ItemTemplate));

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly StyledProperty<double> RowSpacingProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(RowSpacing));

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public static readonly StyledProperty<double> ColumnSpacingProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(ColumnSpacing));

    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public static readonly StyledProperty<ControlTheme> ItemContainerThemeProperty = AvaloniaProperty.Register<CardGrid, ControlTheme>(
        nameof(ItemContainerTheme));

    public ControlTheme ItemContainerTheme
    {
        get => GetValue(ItemContainerThemeProperty);
        set => SetValue(ItemContainerThemeProperty, value);
    }

    public static readonly StyledProperty<double> MinColumnWidthProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(MinColumnWidth), 300d);

    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    public static readonly StyledProperty<double> MaxColumnWidthProperty = AvaloniaProperty.Register<CardGrid, double>(
        nameof(MaxColumnWidth), 500d);

    public double MaxColumnWidth
    {
        get => GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }
}