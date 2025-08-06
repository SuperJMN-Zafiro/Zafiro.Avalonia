using Avalonia.Data;

namespace Zafiro.Avalonia.Controls.Panels;

public class ProportionalCanvas : Panel
{
    public static readonly AttachedProperty<double> LeftProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("Left",
            defaultBindingMode: BindingMode.TwoWay,
            coerce: (Func<AvaloniaObject, double, double>) ((_, val) => Math.Max(0.0, val)));

    public static readonly AttachedProperty<double> ProportionalWidthProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("ProportionalWidth",
            defaultBindingMode: BindingMode.TwoWay,
            coerce: (Func<AvaloniaObject, double, double>) ((_, val) => Math.Max(0.0, val)));

    public static readonly AttachedProperty<double> TopProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("Top",
            defaultBindingMode: BindingMode.TwoWay,
            coerce: (Func<AvaloniaObject, double, double>) ((_, val) => Math.Max(0.0, val)));

    public static readonly AttachedProperty<double> ProportionalHeightProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("ProportionalHeight",
            defaultBindingMode: BindingMode.TwoWay,
            coerce: (Func<AvaloniaObject, double, double>) ((_, val) => Math.Max(0.0, val)));

    public static readonly StyledProperty<double> HorizontalMaximumProperty =
        AvaloniaProperty.Register<ProportionalCanvas, double>(
            nameof(HorizontalMaximum), 1D);

    public static readonly StyledProperty<double> VerticalMaximumProperty =
        AvaloniaProperty.Register<ProportionalCanvas, double>(
            nameof(VerticalMaximum), 1D);

    public double HorizontalMaximum
    {
        get => GetValue(HorizontalMaximumProperty);
        set => SetValue(HorizontalMaximumProperty, value);
    }

    public double VerticalMaximum
    {
        get => GetValue(VerticalMaximumProperty);
        set => SetValue(VerticalMaximumProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            var childRect = GetChildRect(new Vector((float) HorizontalMaximum, (float) VerticalMaximum), finalSize,
                child);
            child.Arrange(childRect);
        }

        return finalSize;
    }

    private static Rect GetChildRect(Vector scale, Size finalSize, Control child)
    {
        var x = child.GetValue(LeftProperty);
        var w = child.GetValue(ProportionalWidthProperty);
        var y = child.GetValue(TopProperty);
        var h = child.GetValue(ProportionalHeightProperty);
        var areaToFill = new Vector(finalSize.Width, (float) finalSize.Height);
        var relativeRect = new Rect(x, y, w, h) / scale;
        return relativeRect * areaToFill;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            var childRect = GetChildRect(new Vector((float) HorizontalMaximum, (float) VerticalMaximum), availableSize,
                child);
            child.Measure(childRect.Size);
        }

        return base.MeasureOverride(availableSize);
    }

    public static void SetLeft(AvaloniaObject target, double value)
    {
        target.SetValue(LeftProperty, value);
    }

    public static double GetLeft(AvaloniaObject target)
    {
        return target.GetValue(LeftProperty);
    }

    public static void SetProportionalWidth(AvaloniaObject target, double value)
    {
        target.SetValue(ProportionalWidthProperty, value);
    }

    public static double GetProportionalWidth(AvaloniaObject target)
    {
        return target.GetValue(ProportionalWidthProperty);
    }

    public static void SetTop(AvaloniaObject target, double value)
    {
        target.SetValue(TopProperty, value);
    }

    public static double GetTop(AvaloniaObject target)
    {
        return target.GetValue(TopProperty);
    }

    public static void SetProportionalHeight(AvaloniaObject target, double value)
    {
        target.SetValue(ProportionalHeightProperty, value);
    }

    public static double GetProportionalHeight(AvaloniaObject target)
    {
        return target.GetValue(ProportionalHeightProperty);
    }
}