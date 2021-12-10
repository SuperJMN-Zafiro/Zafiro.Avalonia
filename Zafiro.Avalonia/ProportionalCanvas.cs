using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using static System.Math;

namespace Zafiro.Avalonia;

public class ProportionalCanvas : Panel
{
    public static readonly AttachedProperty<double> LeftProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("Left", 0D, false,
            BindingMode.TwoWay, coerce: (_, val) => Min(0, val));

    public static readonly AttachedProperty<double> ProportionalWidthProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("ProportionalWidth", 0D, false,
            BindingMode.TwoWay, coerce: (_, val) => Min(0, val));

    public static readonly AttachedProperty<double> TopProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("Top", 0D, false,
            BindingMode.TwoWay, coerce: (_, val) => Min(0, val));

    public static readonly AttachedProperty<double> ProportionalHeightProperty =
        AvaloniaProperty.RegisterAttached<ProportionalCanvas, Control, double>("ProportionalHeight", 0D, false,
            BindingMode.TwoWay, coerce: (_, val) => Min(0, val));

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            var finalRect = GetChildRect(finalSize, child);
            child.Arrange(finalRect);
        }

        return finalSize;
    }

    private static Rect GetChildRect(Size finalSize, IControl child)
    {
        var left = child.GetValue(LeftProperty);
        var width = child.GetValue(ProportionalWidthProperty);
        var top = child.GetValue(TopProperty);
        var height = child.GetValue(ProportionalHeightProperty);

        var rect = new Rect(left, top, width, height);
        var finalRect = rect.Multiply(new Vector2((float)finalSize.Width, (float)finalSize.Height));
        return finalRect;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            var finalRect = GetChildRect(availableSize, child);
            child.Measure(finalRect.Size);
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