
namespace EasyWindowsApplication.Share;

public interface ILayoutBuilderAfterWindow
{
    ILayoutBuilderAfterWindow AlternativeWindow();
    ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure);
}
