
namespace EasyWindowsApplication.Share;

public interface ILayoutBuilder
{
    ILayoutBuilderAfterWindow Window();
    ILayoutBuilderAfterWindow Window(Action<IWindowConfig> configure);
}
