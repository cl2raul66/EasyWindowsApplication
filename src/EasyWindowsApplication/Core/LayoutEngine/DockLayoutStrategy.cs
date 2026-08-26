using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class DockLayoutStrategy : ILayoutStrategy
{
    private readonly IDockLayout? _dock;

    public DockLayoutStrategy(IDockLayout? dock = null)
    {
        _dock = dock;
    }

    public void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight)
    {
        foreach (var child in children)
        {
            child.Measure(availableWidth, availableHeight);
        }
    }

    public void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding)
    {
        float left = padding.Left;
        float top = padding.Top;
        float right = padding.Right;
        float bottom = padding.Bottom;

        float totalW = availableWidth;
        float totalH = availableHeight;

        bool expandLast = _dock?.ShouldExpandLastChild ?? true;
        int lastIndex = children.Count - 1;

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            bool isLast = expandLast && i == lastIndex;

            float childW, childH, childX, childY;

            if (isLast)
            {
                childW = totalW - left - right;
                childH = totalH - top - bottom;
                childX = left;
                childY = top;
            }
            else
            {
                DockPosition dock = child is IDockable d ? d.Dock : (_dock?.DefaultDock ?? DockPosition.Left);

                switch (dock)
                {
                    case DockPosition.Left:
                        childW = child.MeasuredWidth + child.Margin.Left + child.Margin.Right;
                        childH = totalH - top - bottom;
                        childX = left;
                        childY = top;
                        left += childW + spacing;
                        break;

                    case DockPosition.Top:
                        childW = totalW - left - right;
                        childH = child.MeasuredHeight + child.Margin.Top + child.Margin.Bottom;
                        childX = left;
                        childY = top;
                        top += childH + spacing;
                        break;

                    case DockPosition.Right:
                        childW = child.MeasuredWidth + child.Margin.Left + child.Margin.Right;
                        childH = totalH - top - bottom;
                        childX = totalW - right - childW;
                        childY = top;
                        right += childW + spacing;
                        break;

                    case DockPosition.Bottom:
                    default:
                        childW = totalW - left - right;
                        childH = child.MeasuredHeight + child.Margin.Top + child.Margin.Bottom;
                        childX = left;
                        childY = totalH - bottom - childH;
                        bottom += childH + spacing;
                        break;
                }
            }

            float arrangeX = childX + child.Margin.Left;
            float arrangeY = childY + child.Margin.Top;
            float arrangeW = childW - child.Margin.Left - child.Margin.Right;
            float arrangeH = childH - child.Margin.Top - child.Margin.Bottom;

            child.Arrange(
                Math.Max(arrangeX, 0),
                Math.Max(arrangeY, 0),
                Math.Max(arrangeW, 0),
                Math.Max(arrangeH, 0));
        }
    }
}
