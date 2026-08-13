using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IWindowConfig
{
    IWindowConfig Name(string name);
    IWindowConfig Title(string title);
    IWindowConfig Dimensions(int width, int height);
    IWindowConfig Position(WindowPositionOnScreen position);
    IWindowConfig Background(Color color);
    IWindowConfig Scroll(Action<IWindowsScrollConfig> configure);
    IWindowContentConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout;
    IWindowContentConfig Content(Action<IContentBuilder> configure);
}

public interface IWindowContentConfig { }
