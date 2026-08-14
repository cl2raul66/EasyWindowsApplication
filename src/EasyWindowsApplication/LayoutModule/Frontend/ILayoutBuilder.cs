using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Frontend;

public interface ILayoutBuilder
{
    ILayoutBuilderAfterWindow Window();
    ILayoutBuilderAfterWindow Window(Action<IWindowConfig> configure);
}