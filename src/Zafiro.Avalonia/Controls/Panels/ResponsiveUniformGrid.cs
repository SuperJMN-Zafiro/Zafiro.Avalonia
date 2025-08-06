using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Controls.Panels;

public class ResponsiveUniformGrid : Panel
{
    public static readonly StyledProperty<double> MinColumnWidthProperty =
        AvaloniaProperty.Register<ResponsiveUniformGrid, double>(nameof(MinColumnWidth), 250);

    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<ResponsiveUniformGrid, int>(nameof(MaxColumns), int.MaxValue);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<ResponsiveUniformGrid, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<ResponsiveUniformGrid, double>(nameof(RowSpacing), 0d);

    private Size? lastMeasure;

    static ResponsiveUniformGrid()
    {
        AffectsMeasure<ResponsiveUniformGrid>(MinColumnWidthProperty);
        AffectsMeasure<ResponsiveUniformGrid>(MaxColumnsProperty);
        AffectsMeasure<ResponsiveUniformGrid>(ColumnSpacingProperty);
        AffectsMeasure<ResponsiveUniformGrid>(RowSpacingProperty);
    }

    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
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

        int columns = CalculateColumns(availableSize.Width, count);

        double availableCellWidth = double.IsInfinity(availableSize.Width)
            ? double.PositiveInfinity
            : (availableSize.Width - ColumnSpacing * (columns - 1)) / columns;

        double maxCellHeight = 0;
        foreach (var child in Children)
        {
            child.Measure(new Size(availableCellWidth, availableSize.Height));
            maxCellHeight = Math.Max(maxCellHeight, child.DesiredSize.Height);
        }

        int rows = (int)Math.Ceiling(count / (double)columns);
        double totalHeight = maxCellHeight * rows + RowSpacing * (rows - 1);
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

        int columns = CalculateColumns(finalSize.Width, count);
        int rows = (int)Math.Ceiling(count / (double)columns);

        var rawWidth = (finalSize.Width - ColumnSpacing * (columns - 1)) / columns;
        var rawHeight = (finalSize.Height - RowSpacing * (rows - 1)) / rows;
        double cellWidthFinal = double.IsFinite(rawWidth) ? Math.Max(0, rawWidth) : 0;
        double cellHeightFinal = double.IsFinite(rawHeight) ? Math.Max(0, rawHeight) : 0;

        for (int i = 0; i < count; i++)
        {
            int row = i / columns;
            int column = i % columns;

            double x = column * (cellWidthFinal + ColumnSpacing);
            double y = row * (cellHeightFinal + RowSpacing);

            Children[i].Arrange(new Rect(x, y, cellWidthFinal, cellHeightFinal));
        }

        return finalSize;
    }

    private int CalculateColumns(double availableWidth, int count)
    {
        int maxThatFit = Maybe<double>.From(availableWidth)
            .Match(w =>
            {
                if (double.IsInfinity(w) || w <= 0)
                {
                    return Math.Min(MaxColumns, count);
                }

                int possible = Math.Min(MaxColumns, count);
                while (possible > 1)
                {
                    double cellWidth = (w - ColumnSpacing * (possible - 1)) / possible;
                    if (cellWidth >= MinColumnWidth)
                    {
                        break;
                    }

                    possible--;
                }

                return Math.Max(possible, 1);
            }, () => Math.Min(MaxColumns, count));

        return Enumerable.Range(1, maxThatFit)
            .Select(c => new { Columns = c, Holes = c * (int)Math.Ceiling(count / (double)c) - count })
            .OrderBy(x => x.Holes)
            .ThenByDescending(x => x.Columns)
            .First()
            .Columns;
    }
}