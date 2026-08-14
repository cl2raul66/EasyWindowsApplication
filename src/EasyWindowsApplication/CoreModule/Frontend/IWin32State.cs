using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal interface IWin32State
{
    T Get<T>(string name) where T : View<T>;
}
