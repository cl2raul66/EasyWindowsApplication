using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal interface ILayoutStrategy
{
    void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight);
    void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding);
}
