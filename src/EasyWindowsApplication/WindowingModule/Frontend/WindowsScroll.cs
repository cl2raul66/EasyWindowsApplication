using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

internal sealed class WindowsScroll : IWindowsScrollConfig
{
    public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; } = ScrollBarVisibility.Default;
    public ScrollBarVisibility VerticalScrollBarVisibility { get; set; } = ScrollBarVisibility.Default;
    public ScrollOrientation Orientation { get; set; } = ScrollOrientation.Vertical;
    public double ScrollX { get; set; }
    public double ScrollY { get; set; }

    IWindowsScrollConfig IWindowsScrollConfig.HorizontalScrollBarVisibility(ScrollBarVisibility value)
    {
        HorizontalScrollBarVisibility = value;
        return this;
    }

    IWindowsScrollConfig IWindowsScrollConfig.VerticalScrollBarVisibility(ScrollBarVisibility value)
    {
        VerticalScrollBarVisibility = value;
        return this;
    }

    IWindowsScrollConfig IWindowsScrollConfig.Orientation(ScrollOrientation value)
    {
        Orientation = value;
        return this;
    }

    IWindowsScrollConfig IWindowsScrollConfig.ScrollX(double value)
    {
        ScrollX = value;
        return this;
    }

    IWindowsScrollConfig IWindowsScrollConfig.ScrollY(double value)
    {
        ScrollY = value;
        return this;
    }
}
