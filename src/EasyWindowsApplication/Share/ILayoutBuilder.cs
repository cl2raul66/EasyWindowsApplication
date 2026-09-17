
namespace EasyWindowsApplication.Share;

public interface ILayoutBuilder
{
    ILayoutBuilderAfterWindow Window();
    ILayoutBuilderAfterWindow Window(Action<IWindowConfig> configure);
    ILayoutBuilderAfterWindow AlternativeWindow();
    ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure);
    ILayoutBuilderAfterWindow AlternativeWindow<T>(Action<IWindowConfig> configure) where T : class, IViewSurface;
}
