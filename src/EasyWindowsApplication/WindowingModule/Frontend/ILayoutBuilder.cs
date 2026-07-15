using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface ILayoutBuilder
{
    ILayoutBuilder Window(Action<IWindowConfig> configure);
}
