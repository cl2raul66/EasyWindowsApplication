using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal interface ILayoutStrategy
{
    void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight);
    void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding);
}
