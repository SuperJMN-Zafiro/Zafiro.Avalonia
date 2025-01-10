namespace Zafiro.Avalonia.Controls;

using System;
using System.Linq;
using static global::Avalonia.Layout.VerticalAlignment;


/// <summary>
/// Specifies where to position the content
/// </summary>
public enum TrueCenterDock
{
    Left,
    Center,
    Right
}

/// <summary>
/// A panel that arranges its children in three zones: left, center, and right.
/// The center element is always centered relative to the total width of the panel,
/// while being clipped if it overlaps with the side elements.
/// Each element can be vertically aligned independently.
/// </summary>
/// <remarks>
/// This panel is particularly useful for layouts where you need a centered element
/// that maintains its center position relative to the total width, even when
/// side elements might cause overlapping. In such cases, the center element will
/// be clipped while maintaining its centered position.
/// </remarks>
/// <example>
/// <code>
/// <a:TrueCenterPanel>
///     <Button a:TrueCenterPanel.Dock="Left" Content="Left" />
///     <TextBlock a:TrueCenterPanel.Dock="Center" Text="Centered text" TextTrimming="CharacterEllipsis" />
///     <Button a:TrueCenterPanel.Dock="Right" Content="Right" />
/// </a:TrueCenterPanel>
/// </code>
/// </example>
public class TrueCenterPanel : Panel
{
    public static readonly AttachedProperty<TrueCenterDock> DockProperty =
        AvaloniaProperty.RegisterAttached<TrueCenterPanel, Control, TrueCenterDock>(
            "Dock",
            TrueCenterDock.Left
        );

    public static TrueCenterDock GetDock(Control element) 
        => element.GetValue(DockProperty);

    public static void SetDock(Control element, TrueCenterDock value)
        => element.SetValue(DockProperty, value);

    protected override Size MeasureOverride(Size availableSize)
    {
        // Find children (if they exist)
        var leftChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Left);
        var centerChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Center);
        var rightChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Right);

        // First measure the left button with the total width offered by the container
        // (You could limit it somehow, but typically there's no need)
        leftChild?.Measure(availableSize);
        var leftSize = leftChild?.DesiredSize ?? new Size();

        // Also measure the right button
        rightChild?.Measure(availableSize);
        var rightSize = rightChild?.DesiredSize ?? new Size();

        // Calculate remaining space for the center content
        double remainingWidth = availableSize.Width - leftSize.Width - rightSize.Width;
        if (remainingWidth < 0)
            remainingWidth = 0;

        // Now measure the center with that limited space (so it can apply Trimming if needed)
        if (centerChild != null)
        {
            var centerAvailable = new Size(remainingWidth, availableSize.Height);
            centerChild.Measure(centerAvailable);
        }
        var centerSize = centerChild?.DesiredSize ?? new Size();

        // The final height needed by the panel is the maximum of the three children
        double neededHeight = new[] { 
            leftSize.Height, 
            centerSize.Height, 
            rightSize.Height 
        }.Max();

        // Don't exceed available height
        double finalHeight = Math.Min(neededHeight, availableSize.Height);

        // Request full width
        // (If the parent container can't give it all, it will be clipped)
        return new Size(availableSize.Width, finalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var leftChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Left);
        var centerChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Center);
        var rightChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Right);

        var leftSize = leftChild?.DesiredSize ?? new Size();
        var centerSize = centerChild?.DesiredSize ?? new Size();
        var rightSize = rightChild?.DesiredSize ?? new Size();

        // Helper function to calculate vertical position based on alignment
        double GetVerticalPosition(Control? child, double elementHeight)
        {
            if (child == null) return 0;
            
            return child.VerticalAlignment switch
            {
                Top => 0,
                Bottom => Math.Max(0, finalSize.Height - elementHeight),
                Stretch => 0, // For Stretch, start from top
                _ => Math.Max(0, (finalSize.Height - elementHeight) / 2) // Center is default
            };
        }

        // Helper function to get height based on alignment
        double GetVerticalHeight(Control? child, double elementHeight)
        {
            if (child == null) return 0;
            
            return child.VerticalAlignment == Stretch 
                ? Math.Max(1, finalSize.Height)
                : Math.Max(1, Math.Min(elementHeight, finalSize.Height));
        }

        // 1. Left button: x=0
        if (leftChild != null)
        {
            double topLeft = GetVerticalPosition(leftChild, leftSize.Height);
            double height = GetVerticalHeight(leftChild, leftSize.Height);
            
            leftChild.Arrange(new Rect(0, topLeft, 
                Math.Max(0, Math.Min(leftSize.Width, finalSize.Width)),
                height));
        }

        // 2. Right button: aligned to the right
        if (rightChild != null)
        {
            double rightX = Math.Max(0, finalSize.Width - rightSize.Width);
            double topRight = GetVerticalPosition(rightChild, rightSize.Height);
            double height = GetVerticalHeight(rightChild, rightSize.Height);
            
            rightChild.Arrange(new Rect(rightX, topRight, 
                Math.Max(0, Math.Min(rightSize.Width, finalSize.Width - rightX)),
                height));
        }

        // 3. Center: centered relative to total width, but clipped if overlapping
        if (centerChild != null)
        {
            // First calculate ideal centered position relative to total width
            double idealCenterX = Math.Max(0, (finalSize.Width - centerSize.Width) / 2);
            double topCenter = GetVerticalPosition(centerChild, centerSize.Height);
            double height = GetVerticalHeight(centerChild, centerSize.Height);
            
            // Calculate boundaries imposed by side elements
            double leftBoundary = Math.Max(0, leftSize.Width);
            double rightBoundary = Math.Max(leftBoundary, finalSize.Width - rightSize.Width);
            
            // Calculate real available space
            double availableWidth = Math.Max(1, rightBoundary - leftBoundary);
            
            // Determine final width and X position
            double finalWidth;
            double finalX;
            
            if (idealCenterX < leftBoundary)
            {
                finalX = leftBoundary;
                finalWidth = Math.Max(1, Math.Min(centerSize.Width - (leftBoundary - idealCenterX), availableWidth));
            }
            else if (idealCenterX + centerSize.Width > rightBoundary)
            {
                finalWidth = Math.Max(1, rightBoundary - idealCenterX);
                finalX = Math.Min(idealCenterX, rightBoundary - 1);
            }
            else
            {
                finalX = idealCenterX;
                finalWidth = Math.Min(centerSize.Width, rightBoundary - finalX);
            }
            
            // Ensure values are valid
            finalX = Math.Max(0, Math.Min(finalX, finalSize.Width - 1));
            finalWidth = Math.Max(1, Math.Min(finalWidth, finalSize.Width - finalX));
            
            centerChild.Arrange(new Rect(
                finalX,
                topCenter,
                finalWidth,
                height
            ));
        }

        return finalSize;
    }
}