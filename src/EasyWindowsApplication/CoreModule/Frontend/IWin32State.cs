using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IWin32State
{
    T Get<T>(string name) where T : ControlBase<T>;
}
