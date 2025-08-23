// SmartDockPanel - An enhanced version of DockPanel that only applies spacing between visible children
// Reuses the Dock attached property from the original DockPanel

namespace Zafiro.Avalonia.Controls.Panels
{
    /// <summary>
    /// A panel which arranges its children at the top, bottom, left, right or center.
    /// This version only applies spacing between visible children.
    /// </summary>
    public class SmartDockPanel : Panel
    {
        /// <summary>
        /// Defines the <see cref="LastChildFill"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> LastChildFillProperty =
            AvaloniaProperty.Register<SmartDockPanel, bool>(
                nameof(LastChildFill),
                defaultValue: true);

        /// <summary>
        /// Identifies the HorizontalSpacing dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="HorizontalSpacing"/> dependency property.</returns>
        public static readonly StyledProperty<double> HorizontalSpacingProperty =
            AvaloniaProperty.Register<SmartDockPanel, double>(
                nameof(HorizontalSpacing));

        /// <summary>
        /// Identifies the VerticalSpacing dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="VerticalSpacing"/> dependency property.</returns>
        public static readonly StyledProperty<double> VerticalSpacingProperty =
            AvaloniaProperty.Register<SmartDockPanel, double>(
                nameof(VerticalSpacing));

        /// <summary>
        /// Initializes static members of the <see cref="SmartDockPanel"/> class.
        /// </summary>
        static SmartDockPanel()
        {
            // Reuse the DockPanel.DockProperty for this panel
            AffectsParentMeasure<SmartDockPanel>(DockPanel.DockProperty);
            AffectsMeasure<SmartDockPanel>(LastChildFillProperty, HorizontalSpacingProperty, VerticalSpacingProperty);
        }

        /// <summary>
        /// Gets or sets a value which indicates whether the last child of the 
        /// <see cref="SmartDockPanel"/> fills the remaining space in the panel.
        /// </summary>
        public bool LastChildFill
        {
            get => GetValue(LastChildFillProperty);
            set => SetValue(LastChildFillProperty, value);
        }

        /// <summary>
        /// Gets or sets the horizontal distance between the child objects.
        /// </summary>
        public double HorizontalSpacing
        {
            get => GetValue(HorizontalSpacingProperty);
            set => SetValue(HorizontalSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the vertical distance between the child objects.
        /// </summary>
        public double VerticalSpacing
        {
            get => GetValue(VerticalSpacingProperty);
            set => SetValue(VerticalSpacingProperty, value);
        }

        /// <summary>
        /// Updates DesiredSize of the SmartDockPanel.  Called by parent Control.  This is the first pass of layout.
        /// </summary>
        /// <remarks>
        /// Children are measured based on their sizing properties and <see cref="Dock" />.  
        /// Each child is allowed to consume all the space on the side on which it is docked; Left/Right docked
        /// children are granted all vertical space for their entire width, and Top/Bottom docked children are
        /// granted all horizontal space for their entire height.
        /// Spacing is only applied between consecutive visible children.
        /// </remarks>
        /// <param name="availableSize">Constraint size is an "upper limit" that the return value should not exceed.</param>
        /// <returns>The Panel's desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var parentWidth = 0d;
            var parentHeight = 0d;
            var accumulatedWidth = 0d;
            var accumulatedHeight = 0d;

            var hasVisibleHorizontal = false;
            var hasVisibleVertical = false;
            var childrenCount = LastChildFill ? Children.Count - 1 : Children.Count;

            for (var index = 0; index < childrenCount; ++index)
            {
                var child = Children[index];

                if (!child.IsVisible)
                    continue;

                var childConstraint = new Size(
                    Math.Max(0, availableSize.Width - accumulatedWidth),
                    Math.Max(0, availableSize.Height - accumulatedHeight));

                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                switch (DockPanel.GetDock(child))
                {
                    case Dock.Left:
                    case Dock.Right:
                        parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                        // Add spacing before this element if we already have a visible horizontal element
                        if (hasVisibleHorizontal)
                        {
                            accumulatedWidth += HorizontalSpacing;
                        }

                        accumulatedWidth += childDesiredSize.Width;
                        hasVisibleHorizontal = true;
                        break;

                    case Dock.Top:
                    case Dock.Bottom:
                        parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                        // Add spacing before this element if we already have a visible vertical element
                        if (hasVisibleVertical)
                        {
                            accumulatedHeight += VerticalSpacing;
                        }

                        accumulatedHeight += childDesiredSize.Height;
                        hasVisibleVertical = true;
                        break;
                }
            }

            if (LastChildFill && Children.Count > 0)
            {
                var child = Children[Children.Count - 1];

                if (child.IsVisible)
                {
                    // Check what type of dock was used last to apply appropriate spacing
                    bool needsHorizontalSpacing = false;
                    bool needsVerticalSpacing = false;

                    // Look for the last visible docked child to determine spacing
                    for (var i = childrenCount - 1; i >= 0; i--)
                    {
                        if (Children[i].IsVisible)
                        {
                            var lastDock = DockPanel.GetDock(Children[i]);
                            if (lastDock == Dock.Left || lastDock == Dock.Right)
                                needsHorizontalSpacing = true;
                            if (lastDock == Dock.Top || lastDock == Dock.Bottom)
                                needsVerticalSpacing = true;
                            break;
                        }
                    }

                    // Apply spacing if needed
                    if (needsHorizontalSpacing)
                        accumulatedWidth += HorizontalSpacing;
                    if (needsVerticalSpacing)
                        accumulatedHeight += VerticalSpacing;

                    var childConstraint = new Size(
                        Math.Max(0, availableSize.Width - accumulatedWidth),
                        Math.Max(0, availableSize.Height - accumulatedHeight));

                    child.Measure(childConstraint);
                    var childDesiredSize = child.DesiredSize;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                }
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);
            return new Size(parentWidth, parentHeight);
        }

        /// <summary>
        /// SmartDockPanel computes a position and final size for each of its children based upon their
        /// <see cref="Dock" /> enum and sizing properties.
        /// Spacing is only applied between consecutive visible children.
        /// </summary>
        /// <param name="finalSize">Size that SmartDockPanel will assume to position children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count is 0)
                return finalSize;

            var currentBounds = new Rect(finalSize);
            var childrenCount = LastChildFill ? Children.Count - 1 : Children.Count;
            // Flags to control spacing between successive docked elements (orientation based)
            var hasAnyHorizontal = false;
            var hasAnyVertical = false;
            // Flags to know which sides are occupied for spacing around fill element
            var hasLeftDock = false;
            var hasRightDock = false;
            var hasTopDock = false;
            var hasBottomDock = false;

            for (var index = 0; index < childrenCount; ++index)
            {
                var child = Children[index];
                if (!child.IsVisible)
                    continue;

                var dock = DockPanel.GetDock(child);
                double width, height;
                double spacingToApply;

                switch (dock)
                {
                    case Dock.Left:
                        spacingToApply = hasAnyHorizontal ? HorizontalSpacing : 0;
                        currentBounds = new Rect(
                            currentBounds.X + spacingToApply,
                            currentBounds.Y,
                            Math.Max(0, currentBounds.Width - spacingToApply),
                            currentBounds.Height);

                        width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                        child.Arrange(currentBounds.WithWidth(width));

                        currentBounds = new Rect(
                            currentBounds.X + width,
                            currentBounds.Y,
                            Math.Max(0, currentBounds.Width - width),
                            currentBounds.Height);
                        hasAnyHorizontal = true;
                        hasLeftDock = true;
                        break;

                    case Dock.Top:
                        spacingToApply = hasAnyVertical ? VerticalSpacing : 0;
                        currentBounds = new Rect(
                            currentBounds.X,
                            currentBounds.Y + spacingToApply,
                            currentBounds.Width,
                            Math.Max(0, currentBounds.Height - spacingToApply));

                        height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                        child.Arrange(currentBounds.WithHeight(height));

                        currentBounds = new Rect(
                            currentBounds.X,
                            currentBounds.Y + height,
                            currentBounds.Width,
                            Math.Max(0, currentBounds.Height - height));
                        hasAnyVertical = true;
                        hasTopDock = true;
                        break;

                    case Dock.Right:
                        spacingToApply = hasAnyHorizontal ? HorizontalSpacing : 0;
                        currentBounds = currentBounds.WithWidth(Math.Max(0, currentBounds.Width - spacingToApply));

                        width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                        child.Arrange(new Rect(
                            currentBounds.X + currentBounds.Width - width,
                            currentBounds.Y,
                            width,
                            currentBounds.Height));

                        currentBounds = currentBounds.WithWidth(Math.Max(0, currentBounds.Width - width));
                        hasAnyHorizontal = true;
                        hasRightDock = true;
                        break;

                    case Dock.Bottom:
                        spacingToApply = hasAnyVertical ? VerticalSpacing : 0;
                        currentBounds = currentBounds.WithHeight(Math.Max(0, currentBounds.Height - spacingToApply));

                        height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                        child.Arrange(new Rect(
                            currentBounds.X,
                            currentBounds.Y + currentBounds.Height - height,
                            currentBounds.Width,
                            height));

                        currentBounds = currentBounds.WithHeight(Math.Max(0, currentBounds.Height - height));
                        hasAnyVertical = true;
                        hasBottomDock = true;
                        break;
                }
            }

            if (LastChildFill && Children.Count > 0)
            {
                var child = Children[Children.Count - 1];
                if (child.IsVisible)
                {
                    var adjustedBounds = currentBounds;

                    // Horizontal spacing: leave gap next to occupied sides
                    if (hasLeftDock)
                    {
                        adjustedBounds = new Rect(
                            adjustedBounds.X + HorizontalSpacing,
                            adjustedBounds.Y,
                            Math.Max(0, adjustedBounds.Width - HorizontalSpacing),
                            adjustedBounds.Height);
                    }

                    if (hasRightDock)
                    {
                        adjustedBounds = new Rect(
                            adjustedBounds.X,
                            adjustedBounds.Y,
                            Math.Max(0, adjustedBounds.Width - HorizontalSpacing),
                            adjustedBounds.Height);
                    }

                    // Vertical spacing: gap adjacent to top/bottom docked elements.
                    // Top dock consumes space at top, so shift Y down for top gap; bottom gap just reduces height.
                    if (hasTopDock)
                    {
                        adjustedBounds = new Rect(
                            adjustedBounds.X,
                            adjustedBounds.Y + VerticalSpacing,
                            adjustedBounds.Width,
                            Math.Max(0, adjustedBounds.Height - VerticalSpacing));
                    }

                    if (hasBottomDock)
                    {
                        adjustedBounds = new Rect(
                            adjustedBounds.X,
                            adjustedBounds.Y,
                            adjustedBounds.Width,
                            Math.Max(0, adjustedBounds.Height - VerticalSpacing));
                    }

                    child.Arrange(adjustedBounds);
                }
            }

            return finalSize;
        }
    }
}