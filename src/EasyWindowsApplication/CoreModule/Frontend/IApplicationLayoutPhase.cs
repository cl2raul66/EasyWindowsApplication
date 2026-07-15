using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IApplicationLayoutPhase
{
    IApplicationBehaviorPhase Layout();
    IApplicationBehaviorPhase Layout(Action<ILayoutBuilder> configure);
}
