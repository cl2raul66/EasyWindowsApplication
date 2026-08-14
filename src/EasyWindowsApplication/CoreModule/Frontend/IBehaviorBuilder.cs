using System.ComponentModel;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IBehaviorBuilder
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    IBehaviorBuilder OnClick(string controlName, Action handler);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IBehaviorBuilder WithWin32State(Action<IWin32State> configure);

    [EditorBrowsable(EditorBrowsableState.Never)]
    T Get<T>(string name) where T : View<T>;
}
