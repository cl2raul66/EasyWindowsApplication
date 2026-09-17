
namespace EasyWindowsApplication.Share;

public interface ILayoutBuilderAfterWindow
{
    ILayoutBuilderAfterWindow AlternativeWindow();
    ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure);
    ILayoutBuilderAfterWindow AlternativeWindow<T>(Action<IWindowConfig> configure) where T : class, IViewSurface;
}
