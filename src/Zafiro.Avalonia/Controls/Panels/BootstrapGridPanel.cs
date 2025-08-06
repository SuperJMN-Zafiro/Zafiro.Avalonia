using System.ComponentModel;

namespace Zafiro.Avalonia.Controls.Panels;

public class BootstrapGridPanel : Panel
{
    private const int Auto = 0;
    private const int AutoContent = -1;
    private const int NotSet = -2;

    private class ChildInfo
    {
        public Control Control { get; }
        public int Span { get; set; }
        public int Offset { get; }

        public ChildInfo(Control control, int span, int offset)
        {
            Control = control;
            Span = span;
            Offset = offset;
        }
    }

    static BootstrapGridPanel()
    {
        AffectsArrange<BootstrapGridPanel>(
            MaxColumnsProperty,
            GutterProperty,
            FluidContainerProperty);

        AffectsMeasure<BootstrapGridPanel>(
            SmallBreakpointProperty,
            MediumBreakpointProperty,
            LargeBreakpointProperty,
            ExtraLargeBreakpointProperty,
            XXLBreakpointProperty,
            MaxColumnsProperty,
            GutterProperty,
            FluidContainerProperty);

        // Attached properties affect parent layout
        ColProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColSmProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColMdProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColLgProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColXlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColXxlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetSmProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetMdProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetLgProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetXlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetXxlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderSmProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderMdProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderLgProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderXlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderXxlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        RowBreakProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
    }

    private static void InvalidateParentLayout(Control control)
    {
        if (control?.Parent is BootstrapGridPanel panel)
        {
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
    }

    private Breakpoint GetCurrentBreakpoint(double width)
    {
        if (width >= XXLBreakpoint) return Breakpoint.Xxl;
        if (width >= ExtraLargeBreakpoint) return Breakpoint.Xl;
        if (width >= LargeBreakpoint) return Breakpoint.Lg;
        if (width >= MediumBreakpoint) return Breakpoint.Md;
        if (width >= SmallBreakpoint) return Breakpoint.Sm;
        return Breakpoint.Xs;
    }

    private int GetColumnSpan(Control child, Breakpoint breakpoint, double columnWidth, bool measureAuto)
    {
        int span = ResolveColValue(child, breakpoint);

        if (span == AutoContent)
        {
            if (measureAuto)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            var width = child.DesiredSize.Width;
            span = (int)Math.Ceiling((width + Gutter) / (columnWidth + Gutter));
        }

        if (span == Auto)
        {
            return Auto;
        }

        return Math.Min(Math.Max(1, span), MaxColumns);
    }

    private int ResolveColValue(Control child, Breakpoint breakpoint)
    {
        var span = GetCol(child);

        switch (breakpoint)
        {
            case Breakpoint.Xxl:
                span = GetColXxl(child) != NotSet ? GetColXxl(child) : GetColXl(child) != NotSet ? GetColXl(child) : GetColLg(child) != NotSet ? GetColLg(child) : GetColMd(child) != NotSet ? GetColMd(child) : GetColSm(child) != NotSet ? GetColSm(child) : span;
                break;
            case Breakpoint.Xl:
                span = GetColXl(child) != NotSet ? GetColXl(child) : GetColLg(child) != NotSet ? GetColLg(child) : GetColMd(child) != NotSet ? GetColMd(child) : GetColSm(child) != NotSet ? GetColSm(child) : span;
                break;
            case Breakpoint.Lg:
                span = GetColLg(child) != NotSet ? GetColLg(child) : GetColMd(child) != NotSet ? GetColMd(child) : GetColSm(child) != NotSet ? GetColSm(child) : span;
                break;
            case Breakpoint.Md:
                span = GetColMd(child) != NotSet ? GetColMd(child) : GetColSm(child) != NotSet ? GetColSm(child) : span;
                break;
            case Breakpoint.Sm:
                span = GetColSm(child) != NotSet ? GetColSm(child) : span;
                break;
        }

        return span;
    }

    private int GetOffset(Control child, Breakpoint breakpoint)
    {
        var offset = GetOffset(child);

        switch (breakpoint)
        {
            case Breakpoint.Xxl:
                offset = GetOffsetXxl(child) != NotSet ? GetOffsetXxl(child) : GetOffsetXl(child) != NotSet ? GetOffsetXl(child) : GetOffsetLg(child) != NotSet ? GetOffsetLg(child) : GetOffsetMd(child) != NotSet ? GetOffsetMd(child) : GetOffsetSm(child) != NotSet ? GetOffsetSm(child) : offset;
                break;
            case Breakpoint.Xl:
                offset = GetOffsetXl(child) != NotSet ? GetOffsetXl(child) : GetOffsetLg(child) != NotSet ? GetOffsetLg(child) : GetOffsetMd(child) != NotSet ? GetOffsetMd(child) : GetOffsetSm(child) != NotSet ? GetOffsetSm(child) : offset;
                break;
            case Breakpoint.Lg:
                offset = GetOffsetLg(child) != NotSet ? GetOffsetLg(child) : GetOffsetMd(child) != NotSet ? GetOffsetMd(child) : GetOffsetSm(child) != NotSet ? GetOffsetSm(child) : offset;
                break;
            case Breakpoint.Md:
                offset = GetOffsetMd(child) != NotSet ? GetOffsetMd(child) : GetOffsetSm(child) != NotSet ? GetOffsetSm(child) : offset;
                break;
            case Breakpoint.Sm:
                offset = GetOffsetSm(child) != NotSet ? GetOffsetSm(child) : offset;
                break;
        }

        return Math.Min(Math.Max(0, offset), MaxColumns - 1);
    }

    private int GetOrder(Control child, Breakpoint breakpoint)
    {
        var order = GetOrder(child);

        switch (breakpoint)
        {
            case Breakpoint.Xxl:
                order = GetOrderXxl(child) != int.MinValue ? GetOrderXxl(child) : GetOrderXl(child) != int.MinValue ? GetOrderXl(child) : GetOrderLg(child) != int.MinValue ? GetOrderLg(child) : GetOrderMd(child) != int.MinValue ? GetOrderMd(child) : GetOrderSm(child) != int.MinValue ? GetOrderSm(child) : order;
                break;
            case Breakpoint.Xl:
                order = GetOrderXl(child) != int.MinValue ? GetOrderXl(child) : GetOrderLg(child) != int.MinValue ? GetOrderLg(child) : GetOrderMd(child) != int.MinValue ? GetOrderMd(child) : GetOrderSm(child) != int.MinValue ? GetOrderSm(child) : order;
                break;
            case Breakpoint.Lg:
                order = GetOrderLg(child) != int.MinValue ? GetOrderLg(child) : GetOrderMd(child) != int.MinValue ? GetOrderMd(child) : GetOrderSm(child) != int.MinValue ? GetOrderSm(child) : order;
                break;
            case Breakpoint.Md:
                order = GetOrderMd(child) != int.MinValue ? GetOrderMd(child) : GetOrderSm(child) != int.MinValue ? GetOrderSm(child) : order;
                break;
            case Breakpoint.Sm:
                order = GetOrderSm(child) != int.MinValue ? GetOrderSm(child) : order;
                break;
        }

        return order;
    }

    private double GetContainerWidth(double availableWidth)
    {
        if (FluidContainer)
            return availableWidth;

        // Fixed max widths per breakpoint (Bootstrap defaults)
        var breakpoint = GetCurrentBreakpoint(availableWidth);
        return breakpoint switch
        {
            Breakpoint.Sm => Math.Min(540, availableWidth),
            Breakpoint.Md => Math.Min(720, availableWidth),
            Breakpoint.Lg => Math.Min(960, availableWidth),
            Breakpoint.Xl => Math.Min(1140, availableWidth),
            Breakpoint.Xxl => Math.Min(1320, availableWidth),
            _ => availableWidth
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
            return new Size();

        var containerWidth = GetContainerWidth(availableSize.Width);
        var breakpoint = GetCurrentBreakpoint(availableSize.Width);
        var columnWidth = (containerWidth - Gutter * (MaxColumns - 1)) / MaxColumns;

        var orderedChildren = Children
            .OrderBy(c => GetOrder(c, breakpoint))
            .ToList();

        var rows = new List<List<ChildInfo>>();
        var currentRow = new List<ChildInfo>();
        var currentColumn = 0;
        var autoCount = 0;

        foreach (var child in orderedChildren)
        {
            if (GetRowBreak(child) && currentRow.Count > 0)
            {
                MeasureRow(currentRow, columnWidth, availableSize.Height, true);
                rows.Add(currentRow);
                currentRow = new List<ChildInfo>();
                currentColumn = 0;
                autoCount = 0;
            }

            var span = GetColumnSpan(child, breakpoint, columnWidth, true);
            var offset = GetOffset(child, breakpoint);

            if (span == Auto)
            {
                if (currentColumn + offset + autoCount >= MaxColumns && currentRow.Count > 0)
                {
                    MeasureRow(currentRow, columnWidth, availableSize.Height, true);
                    rows.Add(currentRow);
                    currentRow = new List<ChildInfo>();
                    currentColumn = 0;
                    autoCount = 0;
                }

                currentRow.Add(new ChildInfo(child, span, offset));
                autoCount++;
            }
            else
            {
                if (currentColumn + offset + span > MaxColumns && currentRow.Count > 0)
                {
                    MeasureRow(currentRow, columnWidth, availableSize.Height, true);
                    rows.Add(currentRow);
                    currentRow = new List<ChildInfo>();
                    currentColumn = 0;
                    autoCount = 0;
                }

                currentRow.Add(new ChildInfo(child, span, offset));
                currentColumn += offset + span;
            }
        }

        if (currentRow.Count > 0)
        {
            MeasureRow(currentRow, columnWidth, availableSize.Height, true);
            rows.Add(currentRow);
        }

        var totalHeight = rows.Sum(row => row.Max(c => c.Control.DesiredSize.Height)) +
                          Math.Max(0, (rows.Count - 1) * Gutter);

        return new Size(containerWidth, Math.Min(totalHeight, availableSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return finalSize;

        var containerWidth = GetContainerWidth(finalSize.Width);
        var breakpoint = GetCurrentBreakpoint(finalSize.Width);
        var columnWidth = (containerWidth - Gutter * (MaxColumns - 1)) / MaxColumns;

        var containerOffset = FluidContainer ? 0 : (finalSize.Width - containerWidth) / 2;

        var orderedChildren = Children
            .OrderBy(c => GetOrder(c, breakpoint))
            .ToList();

        var rows = new List<List<ChildInfo>>();
        var currentRow = new List<ChildInfo>();
        var currentColumn = 0;
        var autoCount = 0;

        foreach (var child in orderedChildren)
        {
            if (GetRowBreak(child) && currentRow.Count > 0)
            {
                MeasureRow(currentRow, columnWidth, finalSize.Height, false);
                rows.Add(currentRow);
                currentRow = new List<ChildInfo>();
                currentColumn = 0;
                autoCount = 0;
            }

            var span = GetColumnSpan(child, breakpoint, columnWidth, false);
            var offset = GetOffset(child, breakpoint);

            if (span == Auto)
            {
                if (currentColumn + offset + autoCount >= MaxColumns && currentRow.Count > 0)
                {
                    MeasureRow(currentRow, columnWidth, finalSize.Height, false);
                    rows.Add(currentRow);
                    currentRow = new List<ChildInfo>();
                    currentColumn = 0;
                    autoCount = 0;
                }

                currentRow.Add(new ChildInfo(child, span, offset));
                autoCount++;
            }
            else
            {
                if (currentColumn + offset + span > MaxColumns && currentRow.Count > 0)
                {
                    MeasureRow(currentRow, columnWidth, finalSize.Height, false);
                    rows.Add(currentRow);
                    currentRow = new List<ChildInfo>();
                    currentColumn = 0;
                    autoCount = 0;
                }

                currentRow.Add(new ChildInfo(child, span, offset));
                currentColumn += offset + span;
            }
        }

        if (currentRow.Count > 0)
        {
            MeasureRow(currentRow, columnWidth, finalSize.Height, false);
            rows.Add(currentRow);
        }

        var currentY = 0.0;
        foreach (var row in rows)
        {
            var rowHeight = row.Max(ci => ci.Control.DesiredSize.Height);
            var col = 0;
            foreach (var item in row)
            {
                col += item.Offset;
                var x = containerOffset + col * (columnWidth + Gutter);
                var width = columnWidth * item.Span + Gutter * (item.Span - 1);
                item.Control.Arrange(new Rect(x, currentY, width, rowHeight));
                col += item.Span;
            }

            currentY += rowHeight + Gutter;
        }

        return finalSize;
    }

    private void MeasureRow(List<ChildInfo> row, double columnWidth, double availableHeight, bool measure)
    {
        var used = row.Sum(c => c.Offset + (c.Span == Auto ? 0 : c.Span));
        var autos = row.Where(c => c.Span == Auto).ToList();
        var remaining = MaxColumns - used;
        if (autos.Count > 0)
        {
            var per = Math.Max(1, remaining / autos.Count);
            var extra = remaining % autos.Count;
            foreach (var item in autos)
            {
                item.Span = per + (extra-- > 0 ? 1 : 0);
            }
        }

        if (measure)
        {
            foreach (var item in row)
            {
                var width = columnWidth * item.Span + Gutter * (item.Span - 1);
                item.Control.Measure(new Size(width, availableHeight));
            }
        }
    }

    private enum Breakpoint
    {
        Xs,
        Sm,
        Md,
        Lg,
        Xl,
        Xxl
    }

    #region Styled Properties

    // Breakpoints
    public static readonly StyledProperty<double> SmallBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(SmallBreakpoint), 576);

    public static readonly StyledProperty<double> MediumBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(MediumBreakpoint), 768);

    public static readonly StyledProperty<double> LargeBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(LargeBreakpoint), 992);

    public static readonly StyledProperty<double> ExtraLargeBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(ExtraLargeBreakpoint), 1200);

    public static readonly StyledProperty<double> XXLBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(XXLBreakpoint), 1400);

    // Grid configuration
    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, int>(nameof(MaxColumns), 12);

    public static readonly StyledProperty<double> GutterProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(Gutter), 24);

    public static readonly StyledProperty<bool> FluidContainerProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, bool>(nameof(FluidContainer), false);

    #endregion

    #region Attached Properties for Children

    // Column spans for different breakpoints
    public static readonly AttachedProperty<int> ColProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Col", Auto);

    public static readonly AttachedProperty<int> ColSmProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColSm", NotSet);

    public static readonly AttachedProperty<int> ColMdProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColMd", NotSet);

    public static readonly AttachedProperty<int> ColLgProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColLg", NotSet);

    public static readonly AttachedProperty<int> ColXlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColXl", NotSet);

    public static readonly AttachedProperty<int> ColXxlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColXxl", NotSet);

    // Offset properties
    public static readonly AttachedProperty<int> OffsetProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Offset", 0);

    public static readonly AttachedProperty<int> OffsetSmProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetSm", NotSet);

    public static readonly AttachedProperty<int> OffsetMdProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetMd", NotSet);

    public static readonly AttachedProperty<int> OffsetLgProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetLg", NotSet);

    public static readonly AttachedProperty<int> OffsetXlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetXl", NotSet);

    public static readonly AttachedProperty<int> OffsetXxlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetXxl", NotSet);

    // Order properties
    public static readonly AttachedProperty<int> OrderProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Order", 0);

    public static readonly AttachedProperty<int> OrderSmProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OrderSm", int.MinValue);

    public static readonly AttachedProperty<int> OrderMdProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OrderMd", int.MinValue);

    public static readonly AttachedProperty<int> OrderLgProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OrderLg", int.MinValue);

    public static readonly AttachedProperty<int> OrderXlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OrderXl", int.MinValue);

    public static readonly AttachedProperty<int> OrderXxlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OrderXxl", int.MinValue);

    // Row break property
    public static readonly AttachedProperty<bool> RowBreakProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, bool>("RowBreak", false);

    #endregion

    #region Public Properties

    [Category("Layout")]
    public double SmallBreakpoint
    {
        get => GetValue(SmallBreakpointProperty);
        set => SetValue(SmallBreakpointProperty, value);
    }

    [Category("Layout")]
    public double MediumBreakpoint
    {
        get => GetValue(MediumBreakpointProperty);
        set => SetValue(MediumBreakpointProperty, value);
    }

    [Category("Layout")]
    public double LargeBreakpoint
    {
        get => GetValue(LargeBreakpointProperty);
        set => SetValue(LargeBreakpointProperty, value);
    }

    [Category("Layout")]
    public double ExtraLargeBreakpoint
    {
        get => GetValue(ExtraLargeBreakpointProperty);
        set => SetValue(ExtraLargeBreakpointProperty, value);
    }

    [Category("Layout")]
    public double XXLBreakpoint
    {
        get => GetValue(XXLBreakpointProperty);
        set => SetValue(XXLBreakpointProperty, value);
    }

    [Category("Layout")]
    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    [Category("Layout")]
    public double Gutter
    {
        get => GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    [Category("Layout")]
    public bool FluidContainer
    {
        get => GetValue(FluidContainerProperty);
        set => SetValue(FluidContainerProperty, value);
    }

    #endregion

    #region Static Methods for Attached Properties

    public static void SetCol(Control element, int value) => element.SetValue(ColProperty, value);
    public static int GetCol(Control element) => element.GetValue(ColProperty);

    public static void SetColSm(Control element, int value) => element.SetValue(ColSmProperty, value);
    public static int GetColSm(Control element) => element.GetValue(ColSmProperty);

    public static void SetColMd(Control element, int value) => element.SetValue(ColMdProperty, value);
    public static int GetColMd(Control element) => element.GetValue(ColMdProperty);

    public static void SetColLg(Control element, int value) => element.SetValue(ColLgProperty, value);
    public static int GetColLg(Control element) => element.GetValue(ColLgProperty);

    public static void SetColXl(Control element, int value) => element.SetValue(ColXlProperty, value);
    public static int GetColXl(Control element) => element.GetValue(ColXlProperty);

    public static void SetColXxl(Control element, int value) => element.SetValue(ColXxlProperty, value);
    public static int GetColXxl(Control element) => element.GetValue(ColXxlProperty);

    public static void SetColAuto(Control element) => element.SetValue(ColProperty, AutoContent);
    public static void SetColAutoSm(Control element) => element.SetValue(ColSmProperty, AutoContent);
    public static void SetColAutoMd(Control element) => element.SetValue(ColMdProperty, AutoContent);
    public static void SetColAutoLg(Control element) => element.SetValue(ColLgProperty, AutoContent);
    public static void SetColAutoXl(Control element) => element.SetValue(ColXlProperty, AutoContent);
    public static void SetColAutoXxl(Control element) => element.SetValue(ColXxlProperty, AutoContent);

    public static void SetOffset(Control element, int value) => element.SetValue(OffsetProperty, value);
    public static int GetOffset(Control element) => element.GetValue(OffsetProperty);

    public static void SetOffsetSm(Control element, int value) => element.SetValue(OffsetSmProperty, value);
    public static int GetOffsetSm(Control element) => element.GetValue(OffsetSmProperty);

    public static void SetOffsetMd(Control element, int value) => element.SetValue(OffsetMdProperty, value);
    public static int GetOffsetMd(Control element) => element.GetValue(OffsetMdProperty);

    public static void SetOffsetLg(Control element, int value) => element.SetValue(OffsetLgProperty, value);
    public static int GetOffsetLg(Control element) => element.GetValue(OffsetLgProperty);

    public static void SetOffsetXl(Control element, int value) => element.SetValue(OffsetXlProperty, value);
    public static int GetOffsetXl(Control element) => element.GetValue(OffsetXlProperty);

    public static void SetOffsetXxl(Control element, int value) => element.SetValue(OffsetXxlProperty, value);
    public static int GetOffsetXxl(Control element) => element.GetValue(OffsetXxlProperty);

    public static void SetOrder(Control element, int value) => element.SetValue(OrderProperty, value);
    public static int GetOrder(Control element) => element.GetValue(OrderProperty);

    public static void SetOrderSm(Control element, int value) => element.SetValue(OrderSmProperty, value);
    public static int GetOrderSm(Control element) => element.GetValue(OrderSmProperty);

    public static void SetOrderMd(Control element, int value) => element.SetValue(OrderMdProperty, value);
    public static int GetOrderMd(Control element) => element.GetValue(OrderMdProperty);

    public static void SetOrderLg(Control element, int value) => element.SetValue(OrderLgProperty, value);
    public static int GetOrderLg(Control element) => element.GetValue(OrderLgProperty);

    public static void SetOrderXl(Control element, int value) => element.SetValue(OrderXlProperty, value);
    public static int GetOrderXl(Control element) => element.GetValue(OrderXlProperty);

    public static void SetOrderXxl(Control element, int value) => element.SetValue(OrderXxlProperty, value);
    public static int GetOrderXxl(Control element) => element.GetValue(OrderXxlProperty);

    public static void SetRowBreak(Control element, bool value) => element.SetValue(RowBreakProperty, value);
    public static bool GetRowBreak(Control element) => element.GetValue(RowBreakProperty);

    #endregion
}

// Helper class for easier configuration
public static class ResponsiveGrid
{
    public static T Col<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetCol(control, columns);
        return control;
    }

    public static T ColSm<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColSm(control, columns);
        return control;
    }

    public static T ColMd<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColMd(control, columns);
        return control;
    }

    public static T ColLg<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColLg(control, columns);
        return control;
    }

    public static T ColXl<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColXl(control, columns);
        return control;
    }

    public static T ColXxl<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColXxl(control, columns);
        return control;
    }

    public static T ColAuto<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAuto(control);
        return control;
    }

    public static T ColAutoSm<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAutoSm(control);
        return control;
    }

    public static T ColAutoMd<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAutoMd(control);
        return control;
    }

    public static T ColAutoLg<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAutoLg(control);
        return control;
    }

    public static T ColAutoXl<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAutoXl(control);
        return control;
    }

    public static T ColAutoXxl<T>(this T control) where T : Control
    {
        BootstrapGridPanel.SetColAutoXxl(control);
        return control;
    }

    public static T Offset<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffset(control, offset);
        return control;
    }

    public static T OffsetSm<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetSm(control, offset);
        return control;
    }

    public static T OffsetMd<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetMd(control, offset);
        return control;
    }

    public static T OffsetLg<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetLg(control, offset);
        return control;
    }

    public static T OffsetXl<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetXl(control, offset);
        return control;
    }

    public static T OffsetXxl<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetXxl(control, offset);
        return control;
    }

    public static T Order<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrder(control, order);
        return control;
    }

    public static T OrderSm<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrderSm(control, order);
        return control;
    }

    public static T OrderMd<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrderMd(control, order);
        return control;
    }

    public static T OrderLg<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrderLg(control, order);
        return control;
    }

    public static T OrderXl<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrderXl(control, order);
        return control;
    }

    public static T OrderXxl<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrderXxl(control, order);
        return control;
    }

    public static T RowBreak<T>(this T control, bool breakRow = true) where T : Control
    {
        BootstrapGridPanel.SetRowBreak(control, breakRow);
        return control;
    }
}