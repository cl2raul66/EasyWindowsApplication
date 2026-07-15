using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IBehaviorBuilder
{
    IBehaviorBuilder OnClick(string controlName, Action handler);
    IBehaviorBuilder WithWin32State(Action<IWin32State> configure);
    T Get<T>(string name) where T : ControlBase<T>;
}
