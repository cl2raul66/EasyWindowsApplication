using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IWindowsScrollConfig
{
    IWindowsScrollConfig HorizontalScrollBarVisibility(ScrollBarVisibility value);
    IWindowsScrollConfig VerticalScrollBarVisibility(ScrollBarVisibility value);
    IWindowsScrollConfig Orientation(ScrollOrientation value);
    IWindowsScrollConfig ScrollX(double value);
    IWindowsScrollConfig ScrollY(double value);
}
