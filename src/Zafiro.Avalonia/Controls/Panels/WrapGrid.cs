using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Controls.Panels;

/// <summary>
/// A Panel that lays out children in rows similar to a WrapPanel but allocates width inside a row
/// using Grid-like semantics (Pixel, Auto, Star). Children declare their preferred max width via PreferredWidth
/// and an optional minimum width via MinPreferredWidth. Rows are filled sequentially; when adding the next child
/// would exceed the available width (summing minima) a new row is started.
/// </summary>
public class WrapGrid : Panel
{
    // Attached property for preferred (max) width specification (Pixel, Auto, Star)
    public static readonly AttachedProperty<GridLength> PreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>(
            "PreferredWidth", new GridLength(1, GridUnitType.Auto));

    // Attached property for minimum preferred width specification (Pixel, Auto, Star). If not set uses Auto(min of 0 or measured Auto).
    public static readonly AttachedProperty<GridLength> MinPreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>(
            "MinPreferredWidth", new GridLength(0, GridUnitType.Pixel));

    // Nueva propiedad: ancho de relleno cuando hay espacio sobrante en la fila
    public static readonly AttachedProperty<GridLength> FillWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>(
            "FillWidth", new GridLength(0, GridUnitType.Pixel));

    // Attached property: minimum width the item may compress to stay in the current row (before wrapping)
    public static readonly AttachedProperty<double> WrapMinWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, double>("WrapMinWidth", 0d);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(RowSpacing), 0d);

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

    public static void SetPreferredWidth(AvaloniaObject target, GridLength value) => target.SetValue(PreferredWidthProperty, value);
    public static GridLength GetPreferredWidth(AvaloniaObject target) => target.GetValue(PreferredWidthProperty);

    public static void SetMinPreferredWidth(AvaloniaObject target, GridLength value) => target.SetValue(MinPreferredWidthProperty, value);
    public static GridLength GetMinPreferredWidth(AvaloniaObject target) => target.GetValue(MinPreferredWidthProperty);

    public static void SetFillWidth(AvaloniaObject target, GridLength value) => target.SetValue(FillWidthProperty, value);
    public static GridLength GetFillWidth(AvaloniaObject target) => target.GetValue(FillWidthProperty);

    public static void SetWrapMinWidth(AvaloniaObject target, double value) => target.SetValue(WrapMinWidthProperty, value);
    public static double GetWrapMinWidth(AvaloniaObject target) => target.GetValue(WrapMinWidthProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        var availableRowWidth = Maybe<double>.From(availableSize.Width)
            .Where(w => !double.IsInfinity(w))
            .GetValueOrDefault(double.PositiveInfinity);
        var columnSpacing = ColumnSpacing;
        var rowSpacing = RowSpacing;
        var rows = BuildRows(availableRowWidth, availableSize.Height, columnSpacing);

        foreach (var row in rows) AllocateRowWidths(row, availableRowWidth, availableSize.Height, columnSpacing);

        double totalHeight = 0;
        double maxWidth = 0;
        bool firstRow = true;
        foreach (var row in rows)
        {
            if (!firstRow) totalHeight += rowSpacing;
            else firstRow = false;
            totalHeight += row.Height;
            maxWidth = Math.Max(maxWidth, row.Width);
        }

        return new Size(double.IsInfinity(availableSize.Width) ? maxWidth : Math.Min(maxWidth, availableSize.Width),
            double.IsInfinity(availableSize.Height) ? totalHeight : Math.Min(totalHeight, availableSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var availableRowWidth = finalSize.Width;
        var columnSpacing = ColumnSpacing;
        var rowSpacing = RowSpacing;
        var rows = BuildRows(availableRowWidth, finalSize.Height, columnSpacing);

        foreach (var row in rows) AllocateRowWidths(row, availableRowWidth, finalSize.Height, columnSpacing);

        double y = 0;
        bool first = true;
        foreach (var row in rows)
        {
            if (!first) y += rowSpacing;
            else first = false;
            double x = 0;
            double rowHeight = row.Height;
            int idx = 0;
            foreach (var item in row.Items)
            {
                if (idx > 0) x += columnSpacing;
                item.Child.Arrange(new Rect(x, y, item.AllocatedWidth, rowHeight));
                x += item.AllocatedWidth;
                idx++;
            }

            y += rowHeight;
        }

        return finalSize;
    }

    private List<Row> BuildRows(double availableRowWidth, double availableHeight, double columnSpacing)
    {
        var rows = new List<Row>();
        var current = new Row();
        rows.Add(current);
        double currentMin = 0;

        foreach (var child in Children)
        {
            if (child is not Control control || !control.IsVisible)
                continue;

            var item = CreateItem(control, availableHeight);
            double itemMin = GetItemMinContribution(item);
            double required = currentMin + (current.Items.Count > 0 ? columnSpacing : 0) + itemMin;

            if (!double.IsInfinity(availableRowWidth) && current.Items.Count > 0 && required > availableRowWidth)
            {
                current = new Row();
                rows.Add(current);
                currentMin = 0;
                required = itemMin;
            }

            if (current.Items.Count > 0)
                currentMin += columnSpacing;
            currentMin += itemMin;
            current.Items.Add(item);
        }

        return rows;
    }

    private Item CreateItem(Control c, double availableHeight)
    {
        var pref = GetPreferredWidth(c);
        var minPref = GetMinPreferredWidth(c);
        var fill = GetFillWidth(c);
        c.Measure(new Size(double.PositiveInfinity, availableHeight));
        double autoWidth = c.DesiredSize.Width;
        var wrapMin = GetWrapMinWidth(c);
        var item = new Item
        {
            Child = c,
            Preferred = pref,
            MinPreferred = minPref,
            Fill = fill,
            AutoDesiredWidth = autoWidth,
            WrapMinWidth = wrapMin
        };
        return item;
    }

    private static double GetItemMinContribution(Item item)
    {
        if (item.Preferred.IsAbsolute) return item.Preferred.Value;
        if (item.WrapMinWidth > 0) return item.WrapMinWidth;
        if (item.Preferred.IsAuto) return item.AutoDesiredWidth;
        return 0;
    }


    private void AllocateRowWidths(Row row, double availableRowWidth, double availableHeight, double spacing)
    {
        double fixedSum = 0;
        double totalStar = 0;
        foreach (var item in row.Items)
        {
            if (item.Preferred.IsAbsolute) fixedSum += item.Preferred.Value;
            else if (item.Preferred.IsAuto) fixedSum += item.AutoDesiredWidth;
            else if (item.Preferred.IsStar) totalStar += Math.Max(item.Preferred.Value, 0.0001);
        }

        int gapCount = Math.Max(0, row.Items.Count - 1);
        double spacingTotal = gapCount * spacing;
        double leftover = double.IsInfinity(availableRowWidth) ? fixedSum : availableRowWidth - fixedSum - spacingTotal;
        if (leftover < 0) leftover = 0;
        foreach (var item in row.Items)
        {
            double width = item.Preferred.IsAbsolute ? item.Preferred.Value : item.Preferred.IsAuto ? item.AutoDesiredWidth : (totalStar > 0 ? leftover * (item.StarWeight / totalStar) : 0);
            item.AllocatedWidth = width;
            item.Child.Measure(new Size(width, availableHeight));
            item.DesiredHeight = item.Child.DesiredSize.Height;
        }

        double baseRowWidth = spacingTotal;
        foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
        if (!double.IsInfinity(availableRowWidth) && baseRowWidth > availableRowWidth + 0.1)
        {
            double over = baseRowWidth - availableRowWidth;
            var compressible = new List<Item>();
            double totalSlack = 0;
            foreach (var it in row.Items)
            {
                if (it.Preferred.IsAbsolute) continue;
                double min = it.WrapMinWidth > 0 ? it.WrapMinWidth : 0;
                double slack = it.AllocatedWidth - min;
                if (slack > 0.1)
                {
                    compressible.Add(it);
                    totalSlack += slack;
                }
            }

            if (totalSlack > 0)
            {
                foreach (var it in compressible)
                {
                    double min = it.WrapMinWidth > 0 ? it.WrapMinWidth : 0;
                    double slack = it.AllocatedWidth - min;
                    double reduce = Math.Min(slack, over * (slack / totalSlack));
                    it.AllocatedWidth -= reduce;
                    over -= reduce;
                }
            }

            baseRowWidth = spacingTotal;
            foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
        }

        double spare = double.IsInfinity(availableRowWidth) ? 0 : Math.Max(0, availableRowWidth - baseRowWidth);
        if (spare > 0.1)
        {
            var pixelAutoList = new List<(Item item, double needed)>();
            foreach (var it in row.Items)
            {
                var fill = it.Fill;
                if (fill.GridUnitType == GridUnitType.Pixel && fill.Value > 0)
                {
                    double target = fill.Value;
                    if (target > it.AllocatedWidth) pixelAutoList.Add((it, target - it.AllocatedWidth));
                }
                else if (fill.GridUnitType == GridUnitType.Auto)
                {
                    double target = Math.Max(it.AutoDesiredWidth, it.AllocatedWidth);
                    double needed = target - it.AllocatedWidth;
                    if (needed > 0.1) pixelAutoList.Add((it, needed));
                }
            }

            double totalNeeded = 0;
            foreach (var pa in pixelAutoList) totalNeeded += pa.needed;
            if (totalNeeded > 0 && spare > 0)
            {
                foreach (var pa in pixelAutoList)
                {
                    double grant = Math.Min(pa.needed, spare * (pa.needed / totalNeeded));
                    pa.item.AllocatedWidth += grant;
                    spare -= grant;
                }
            }

            if (spare > 0.1)
            {
                double fillStarSum = 0;
                foreach (var it in row.Items)
                    if (it.Fill.IsStar)
                        fillStarSum += Math.Max(it.Fill.Value, 0.0001);
                if (fillStarSum > 0)
                {
                    foreach (var it in row.Items)
                    {
                        if (it.Fill.IsStar)
                        {
                            double add = spare * (it.Fill.Value / fillStarSum);
                            it.AllocatedWidth += add;
                        }
                    }
                }
            }
        }

        double rowHeight = 0;
        double rowWidth = spacingTotal;
        foreach (var item in row.Items)
        {
            item.Child.Measure(new Size(item.AllocatedWidth, availableHeight));
            item.DesiredHeight = Math.Max(item.DesiredHeight, item.Child.DesiredSize.Height);
            rowHeight = Math.Max(rowHeight, item.DesiredHeight);
            rowWidth += item.AllocatedWidth;
        }

        row.Height = rowHeight;
        row.Width = rowWidth;
    }

    private class Row
    {
        public double Height; // calculated
        public double Width; // calculated
        public List<Item> Items { get; } = new();
    }

    private class Item
    {
        public double AllocatedWidth; // final width
        public double AutoDesiredWidth; // width measured with infinite constraint (for Auto)
        public Control Child = null!;
        public double DesiredHeight; // after final measure
        public GridLength Fill; // fill specification
        public GridLength MinPreferred;
        public GridLength Preferred;
        public double WrapMinWidth; // minimum width allowed before forcing new row
        public double StarWeight => Preferred.IsStar ? Preferred.Value : 0d;
    }
}