using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal interface ILayoutable
{
    LayoutLength? LayoutWidth { get; }
    LayoutLength? LayoutHeight { get; }
    LayoutOptions LayoutOptions { get; }
    Thickness Margin { get; }
    Thickness Padding { get; }

    float MeasuredWidth { get; }
    float MeasuredHeight { get; }

    void Measure(float availableWidth, float availableHeight);
    void Arrange(float x, float y, float width, float height);
    void Render();
}
