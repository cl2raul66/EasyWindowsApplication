using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IWin32State
{
    T Get<T>(string name) where T : View<T>;
}
