
namespace EasyWindowsApplication.Share;

public interface IApplicationLayoutPhase
{
    IApplicationPostLayoutPhase Layout(Action<ILayoutBuilder> configure);
}
