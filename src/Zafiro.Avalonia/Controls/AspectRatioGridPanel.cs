using CSharpFunctionalExtensions;
using System.Linq;

namespace Zafiro.Avalonia.Controls;

public class AspectRatioGridPanel : Panel
{
    private Size? lastMeasure;

    public static readonly StyledProperty<double> AspectRatioProperty =
        AvaloniaProperty.Register<AspectRatioGridPanel, double>(nameof(AspectRatio), 1.0);

    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<AspectRatioGridPanel, int>(nameof(MaxColumns), int.MaxValue);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<AspectRatioGridPanel, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<AspectRatioGridPanel, double>(nameof(RowSpacing), 0d);

    static AspectRatioGridPanel()
    {
        AffectsMeasure<AspectRatioGridPanel>(AspectRatioProperty);
        AffectsMeasure<AspectRatioGridPanel>(MaxColumnsProperty);
        AffectsMeasure<AspectRatioGridPanel>(ColumnSpacingProperty);
        AffectsMeasure<AspectRatioGridPanel>(RowSpacingProperty);
    }

    public double AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        int count = Children.Count;
        if (count == 0)
        {
            return new Size();
        }

        int columns = CalculateColumns(count);

        double availableCellWidth = Maybe<double>.From(availableSize.Width)
            .Match(w => double.IsInfinity(w)
                ? double.PositiveInfinity
                : (w - ColumnSpacing * (columns - 1)) / columns, () => double.PositiveInfinity);

        double availableCellHeight = availableCellWidth / AspectRatio;

        foreach (var child in Children)
        {
            child.Measure(new Size(availableCellWidth, availableCellHeight));
        }

        int rows = (int)Math.Ceiling(count / (double)columns);
        double totalHeight = availableCellHeight * rows + RowSpacing * (rows - 1);
        double totalWidth = availableCellWidth * columns + ColumnSpacing * (columns - 1);

        var result = new Size(totalWidth, totalHeight);
        lastMeasure = result;
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (double.IsInfinity(finalSize.Width) || double.IsInfinity(finalSize.Height))
        {
            finalSize = lastMeasure ?? finalSize;
        }

        int count = Children.Count;
        if (count == 0)
        {
            return finalSize;
        }

        int columns = CalculateColumns(count);
        int rows = (int)Math.Ceiling(count / (double)columns);

        var rawWidth = (finalSize.Width - ColumnSpacing * (columns - 1)) / columns;
        var cellWidth = double.IsFinite(rawWidth) ? Math.Max(0, rawWidth) : 0;
        var cellHeight = cellWidth / AspectRatio;

        for (int i = 0; i < count; i++)
        {
            int row = i / columns;
            int column = i % columns;

            double x = column * (cellWidth + ColumnSpacing);
            double y = row * (cellHeight + RowSpacing);

            Children[i].Arrange(new Rect(x, y, cellWidth, cellHeight));
        }

        return finalSize;
    }

    private int CalculateColumns(int count)
    {
        int possible = Math.Min(MaxColumns, count);

        return Enumerable.Range(1, possible)
            .Select(c => new { Columns = c, Holes = c * (int)Math.Ceiling(count / (double)c) - count })
            .OrderBy(x => x.Holes)
            .ThenBy(x => x.Columns)
            .First()
            .Columns;
    }
}

