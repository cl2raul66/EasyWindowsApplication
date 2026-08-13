using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class VerticalStackLayoutStrategy : ILayoutStrategy
{
    public void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight)
    {
        foreach (var child in children)
        {
            float childAvailW = availableWidth - child.Margin.Left - child.Margin.Right;
            float childAvailH = availableHeight;
            child.Measure(Math.Max(childAvailW, 0), childAvailH);
        }
    }

    public void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding)
    {
        // ── 1st pass: measure Auto / Absolute children and accumulate height ──
        float totalFixedHeight = 0;
        float totalStarWeight = 0;

        foreach (var child in children)
        {
            bool isHeightStar = child.LayoutHeight?.Type is GridUnitType.Star;
            if (!isHeightStar)
            {
                totalFixedHeight += child.MeasuredHeight + child.Margin.Top + child.Margin.Bottom;
            }
            else
            {
                totalStarWeight += child.LayoutHeight!.Value.Value;
            }
        }

        if (children.Count > 1)
            totalFixedHeight += spacing * (children.Count - 1);

        float remainingHeight = availableHeight - totalFixedHeight;

        // ── 2nd pass: assign positions ──
        float y = padding.Top;

        foreach (var child in children)
        {
            bool isHeightStar = child.LayoutHeight?.Type is GridUnitType.Star;
            float childH;

            if (isHeightStar && totalStarWeight > 0)
            {
                float starValue = child.LayoutHeight!.Value.Value;
                float proportion = starValue / totalStarWeight;
                childH = (int)(proportion * remainingHeight);
                childH -= child.Margin.Top + child.Margin.Bottom;
            }
            else
            {
                childH = child.MeasuredHeight;
            }

            childH = Math.Max(childH, 0);

            // Determine width based on alignment
            float childW;
            float childX = padding.Left;

            bool isWidthStar = child.LayoutWidth?.Type is GridUnitType.Star;
            bool isWidthAuto = child.LayoutWidth?.Type is GridUnitType.Auto;
            bool isWidthAbsolute = child.LayoutWidth?.Type is GridUnitType.Absolute;

            if (isWidthStar || child.LayoutOptions.HorizontalAlignment is LayoutAlignment.Fill)
            {
                childW = availableWidth - child.Margin.Left - child.Margin.Right;
            }
            else if (isWidthAbsolute)
            {
                childW = Math.Min(child.LayoutWidth!.Value.Value, availableWidth - child.Margin.Left - child.Margin.Right);
            }
            else
            {
                childW = Math.Min(child.MeasuredWidth, availableWidth - child.Margin.Left - child.Margin.Right);
            }

            childW = Math.Max(childW, 0);

            // Horizontal alignment (only meaningful when child isn't filling)
            if (child.LayoutOptions.HorizontalAlignment is LayoutAlignment.Center &&
                childW < availableWidth - child.Margin.Left - child.Margin.Right)
            {
                float slotWidth = availableWidth - child.Margin.Left - child.Margin.Right;
                childX = padding.Left + child.Margin.Left + (slotWidth - childW) / 2;
            }
            else if (child.LayoutOptions.HorizontalAlignment is LayoutAlignment.End &&
                     childW < availableWidth - child.Margin.Left - child.Margin.Right)
            {
                childX = padding.Left + availableWidth - childW - child.Margin.Right;
            }
            else
            {
                childX = padding.Left + child.Margin.Left;
            }

            child.Arrange(childX, y + child.Margin.Top, childW, childH);
            y += childH + child.Margin.Top + child.Margin.Bottom + spacing;
        }
    }
}
