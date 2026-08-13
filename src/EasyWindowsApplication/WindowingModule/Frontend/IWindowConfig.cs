using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IWindowConfig
{
    IWindowConfig Name(string name);
    IWindowConfig Title(string title);
    IWindowConfig Dimensions(int width, int height);
    IWindowConfig Position(WindowPosition position);
    IWindowConfig Content(Action<IContentBuilder> configure);
    IWindowConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout;
}
