
namespace EasyWindowsApplication.Share;

public interface ILayoutBuilderAfterWindow
{
    ILayoutBuilderAfterWindow AlternativeWindow();
    ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure);
    ILayoutBuilderAfterWindow AlternativeWindow<T>(Action<IWindowConfig> configure) where T : class, IViewSurface;
    ILayoutBuilderAfterWindow AlternativeWindow<TDialog>(Action<TDialog> configure) where TDialog : class, ISystemDialog;
}
