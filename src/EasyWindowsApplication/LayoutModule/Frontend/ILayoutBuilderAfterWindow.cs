using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Frontend;

public interface ILayoutBuilderAfterWindow
{
    ILayoutBuilderAfterWindow AlternativeWindow();
    ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure);
    ILayoutBuilderAfterWindow Window();
    ILayoutBuilderAfterWindow Window(Action<IWindowConfig> configure);
}
