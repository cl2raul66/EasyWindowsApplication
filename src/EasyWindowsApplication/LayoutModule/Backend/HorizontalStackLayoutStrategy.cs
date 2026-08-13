using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class HorizontalStackLayoutStrategy : ILayoutStrategy
{
    public void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight)
    {
        foreach (var child in children)
        {
            float childAvailH = availableHeight - child.Margin.Top - child.Margin.Bottom;
            child.Measure(availableWidth, Math.Max(childAvailH, 0));
        }
    }

    public void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding)
    {
        float totalFixedWidth = 0;
        float totalStarWeight = 0;

        foreach (var child in children)
        {
            bool isWidthStar = child.LayoutWidth?.Type is GridUnitType.Star;
            if (!isWidthStar)
            {
                totalFixedWidth += child.MeasuredWidth + child.Margin.Left + child.Margin.Right;
            }
            else
            {
                totalStarWeight += child.LayoutWidth!.Value.Value;
            }
        }

        if (children.Count > 1)
            totalFixedWidth += spacing * (children.Count - 1);

        float remainingWidth = availableWidth - totalFixedWidth;

        float x = padding.Left;

        foreach (var child in children)
        {
            bool isWidthStar = child.LayoutWidth?.Type is GridUnitType.Star;
            float childW;

            if (isWidthStar && totalStarWeight > 0)
            {
                float proportion = child.LayoutWidth!.Value.Value / totalStarWeight;
                childW = proportion * remainingWidth - child.Margin.Left - child.Margin.Right;
            }
            else
            {
                childW = child.MeasuredWidth;
            }

            childW = Math.Max(childW, 0);

            float slotHeight = availableHeight - child.Margin.Top - child.Margin.Bottom;
            float childH;

            if (child.LayoutOptions.VerticalAlignment is LayoutAlignment.Fill)
                childH = slotHeight;
            else
                childH = Math.Min(child.MeasuredHeight, slotHeight);

            childH = Math.Max(childH, 0);

            float childY = padding.Top + child.Margin.Top;

            if (child.LayoutOptions.VerticalAlignment is LayoutAlignment.Center && childH < slotHeight)
            {
                childY = padding.Top + child.Margin.Top + (slotHeight - childH) / 2;
            }
            else if (child.LayoutOptions.VerticalAlignment is LayoutAlignment.End && childH < slotHeight)
            {
                childY = padding.Top + availableHeight - childH - child.Margin.Bottom;
            }

            child.Arrange(x + child.Margin.Left, childY, childW, childH);
            x += childW + child.Margin.Left + child.Margin.Right + spacing;
        }
    }
}
