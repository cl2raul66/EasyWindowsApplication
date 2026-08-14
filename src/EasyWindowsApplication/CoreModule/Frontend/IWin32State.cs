using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IWin32State
{
    T Get<T>(string name) where T : View<T>;
}
