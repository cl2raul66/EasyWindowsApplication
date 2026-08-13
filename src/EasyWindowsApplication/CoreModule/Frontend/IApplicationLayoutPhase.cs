using EasyWindowsApplication.LayoutModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IApplicationLayoutPhase
{
    IApplicationPostLayoutPhase Layout();
    IApplicationPostLayoutPhase Layout(Action<ILayoutBuilder> configure);
}