using System.Buffers;

namespace Zafiro.Avalonia.Controls.Panels;

/// <summary>
/// Optimized WrapGrid: lays out children in rows with Grid-like width semantics (Pixel, Auto, Star).
/// </summary>
public class WrapGrid : Panel
{
    // Attached properties
    public static readonly AttachedProperty<GridLength> PreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("PreferredWidth", new GridLength(1, GridUnitType.Auto));

    public static readonly AttachedProperty<GridLength> MinPreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("MinPreferredWidth", new GridLength(0, GridUnitType.Pixel));

    public static readonly AttachedProperty<GridLength> FillWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("FillWidth", new GridLength(0, GridUnitType.Pixel));

    public static readonly AttachedProperty<double> WrapMinWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, double>("WrapMinWidth", 0d);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(RowSpacing), 0d);

    // Simple cache for Auto desired widths (infinite constraint)
    private readonly Dictionary<Control, double> autoWidthCache = new();

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
        if (Children.Count == 0)
            return new Size();

        double availableRowWidth = double.IsInfinity(availableSize.Width) ? double.PositiveInfinity : availableSize.Width;
        double colSpacing = ColumnSpacing;
        double rowSpacing = RowSpacing;

        var rows = BuildRows(availableRowWidth, availableSize.Height);

        foreach (var row in rows)
            AllocateRowWidths(row, availableRowWidth, availableSize.Height, colSpacing);

        double totalHeight = 0;
        double maxWidth = 0;
        bool first = true;
        foreach (var r in rows)
        {
            if (!first) totalHeight += rowSpacing;
            else first = false;
            totalHeight += r.Height;
            maxWidth = Math.Max(maxWidth, r.Width);
        }

        double finalWidth = double.IsInfinity(availableSize.Width) ? maxWidth : Math.Min(maxWidth, availableSize.Width);
        double finalHeight = double.IsInfinity(availableSize.Height) ? totalHeight : Math.Min(totalHeight, availableSize.Height);
        return new Size(finalWidth, finalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return finalSize;

        double availableRowWidth = finalSize.Width;
        double colSpacing = ColumnSpacing;
        double rowSpacing = RowSpacing;

        var rows = BuildRows(availableRowWidth, finalSize.Height);
        foreach (var row in rows)
            AllocateRowWidths(row, availableRowWidth, finalSize.Height, colSpacing);

        double y = 0;
        bool first = true;
        foreach (var row in rows)
        {
            if (!first) y += rowSpacing;
            else first = false;
            double x = 0;
            int i = 0;
            foreach (var item in row.Items)
            {
                if (i > 0) x += colSpacing;
                item.Child.Arrange(new Rect(x, y, item.AllocatedWidth, row.Height));
                x += item.AllocatedWidth;
                i++;
            }

            y += row.Height;
        }

        return finalSize;
    }

    // Build logical rows (only needs minimal width info)
    private List<Row> BuildRows(double availableRowWidth, double availableHeight)
    {
        var rows = new List<Row>(4);
        var current = new Row();
        rows.Add(current);
        double currentMin = 0;
        double colSpacing = ColumnSpacing;

        foreach (var child in Children)
        {
            if (child is not Control c || !c.IsVisible)
                continue;

            var item = CreateItem(c, availableHeight);
            double itemMin = GetItemMinContribution(item);
            double required = currentMin + (current.Items.Count > 0 ? colSpacing : 0) + itemMin;

            if (!double.IsInfinity(availableRowWidth) && current.Items.Count > 0 && required > availableRowWidth)
            {
                current = new Row();
                rows.Add(current);
                currentMin = 0;
                required = itemMin;
            }

            if (current.Items.Count > 0)
                currentMin += colSpacing;
            currentMin += itemMin;
            current.Items.Add(item);
        }

        return rows;
    }

    private Item CreateItem(Control c, double availableHeight)
    {
        var preferred = GetPreferredWidth(c);
        var minPreferred = GetMinPreferredWidth(c);
        var fill = GetFillWidth(c);
        double autoDesired = 0;

        // Only measure now if we truly need unconstrained width (Auto semantics)
        if (preferred.IsAuto || fill.IsAuto)
        {
            if (!autoWidthCache.TryGetValue(c, out autoDesired))
            {
                c.Measure(new Size(double.PositiveInfinity, availableHeight));
                autoDesired = c.DesiredSize.Width;
                autoWidthCache[c] = autoDesired;
            }
        }

        return new Item
        {
            Child = c,
            Preferred = preferred,
            MinPreferred = minPreferred,
            Fill = fill,
            AutoDesiredWidth = autoDesired,
            WrapMinWidth = GetWrapMinWidth(c)
        };
    }

    private static double GetItemMinContribution(Item item)
    {
        if (item.Preferred.IsAbsolute)
            return item.Preferred.Value;
        if (item.WrapMinWidth > 0)
            return item.WrapMinWidth;
        if (item.Preferred.IsAuto)
            return item.AutoDesiredWidth;
        return 0;
    }

    // Assign widths (single final measure pass at end)
    private void AllocateRowWidths(Row row, double availableRowWidth, double availableHeight, double spacing)
    {
        if (row.Items.Count == 0)
        {
            row.Height = 0;
            row.Width = 0;
            return;
        }

        double fixedSum = 0;
        double totalStar = 0;

        // First pass: compute fixed and star aggregate
        foreach (var item in row.Items)
        {
            var pref = item.Preferred;
            if (pref.IsAbsolute)
                fixedSum += pref.Value;
            else if (pref.IsAuto)
                fixedSum += item.AutoDesiredWidth;
            else if (pref.IsStar)
                totalStar += Math.Max(pref.Value, 0.0001);
        }

        int gaps = row.Items.Count - 1;
        double spacingTotal = gaps > 0 ? gaps * spacing : 0;
        double leftover = double.IsInfinity(availableRowWidth) ? fixedSum : availableRowWidth - fixedSum - spacingTotal;
        if (leftover < 0) leftover = 0;

        // Assign initial widths (no Measure here)
        foreach (var item in row.Items)
        {
            double width;
            if (item.Preferred.IsAbsolute)
                width = item.Preferred.Value;
            else if (item.Preferred.IsAuto)
                width = item.AutoDesiredWidth;
            else // Star
                width = totalStar > 0 ? leftover * (item.Preferred.Value / totalStar) : 0;
            item.AllocatedWidth = width;
        }

        double baseRowWidth = spacingTotal;
        foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;

        // Compression if overflow
        if (!double.IsInfinity(availableRowWidth) && baseRowWidth > availableRowWidth + 0.1)
        {
            double over = baseRowWidth - availableRowWidth;
            var buffer = ArrayPool<Item>.Shared.Rent(row.Items.Count);
            int count = 0;
            double totalSlack = 0;
            foreach (var it in row.Items)
            {
                if (it.Preferred.IsAbsolute)
                    continue;
                double min = it.WrapMinWidth > 0 ? it.WrapMinWidth : 0;
                double slack = it.AllocatedWidth - min;
                if (slack > 0.1)
                {
                    buffer[count++] = it;
                    totalSlack += slack;
                }
            }

            if (totalSlack > 0)
            {
                for (int i = 0; i < count && over > 0.01; i++)
                {
                    var it = buffer[i];
                    double min = it.WrapMinWidth > 0 ? it.WrapMinWidth : 0;
                    double slack = it.AllocatedWidth - min;
                    if (slack <= 0) continue;
                    double reduce = Math.Min(slack, over * (slack / totalSlack));
                    it.AllocatedWidth -= reduce;
                    over -= reduce;
                }
            }

            ArrayPool<Item>.Shared.Return(buffer);
            baseRowWidth = spacingTotal;
            foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
        }

        // Fill (Pixel / Auto / Star) only if spare space
        double spare = double.IsInfinity(availableRowWidth) ? 0 : Math.Max(0, availableRowWidth - baseRowWidth);
        if (spare > 0.1)
        {
            // Phase 1: Pixel + Auto fill
            var fillCandidates = ArrayPool<(Item item, double need)>.Shared.Rent(row.Items.Count);
            int neededCount = 0;
            double totalNeed = 0;
            foreach (var it in row.Items)
            {
                var fill = it.Fill;
                if (fill.GridUnitType == GridUnitType.Pixel && fill.Value > it.AllocatedWidth)
                {
                    double need = fill.Value - it.AllocatedWidth;
                    fillCandidates[neededCount++] = (it, need);
                    totalNeed += need;
                }
                else if (fill.GridUnitType == GridUnitType.Auto)
                {
                    double target = Math.Max(it.AutoDesiredWidth, it.AllocatedWidth);
                    double need = target - it.AllocatedWidth;
                    if (need > 0.1)
                    {
                        fillCandidates[neededCount++] = (it, need);
                        totalNeed += need;
                    }
                }
            }

            if (totalNeed > 0)
            {
                for (int i = 0; i < neededCount && spare > 0.01; i++)
                {
                    var pair = fillCandidates[i];
                    double grant = Math.Min(pair.need, spare * (pair.need / totalNeed));
                    pair.item.AllocatedWidth += grant;
                    spare -= grant;
                }
            }

            ArrayPool<(Item, double)>.Shared.Return(fillCandidates);

            // Phase 2: Star fill
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
                            it.AllocatedWidth += spare * (it.Fill.Value / fillStarSum);
                    }

                    baseRowWidth = spacingTotal;
                    foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
                    spare = 0;
                }
            }
        }

        // Final measure only once per child
        double rowHeight = 0;
        foreach (var item in row.Items)
        {
            item.Child.Measure(new Size(item.AllocatedWidth, availableHeight));
            double h = item.Child.DesiredSize.Height;
            if (h > rowHeight) rowHeight = h;
        }

        row.Height = rowHeight;
        row.Width = spacingTotal;
        foreach (var it in row.Items) row.Width += it.AllocatedWidth;
    }

    private class Row
    {
        public double Height;
        public double Width;
        public List<Item> Items { get; } = new(8);
    }

    private class Item
    {
        public double AllocatedWidth;
        public double AutoDesiredWidth;
        public Control Child = null!;
        public GridLength Fill;
        public GridLength MinPreferred;
        public GridLength Preferred;
        public double WrapMinWidth;
    }
}