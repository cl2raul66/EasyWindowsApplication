using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class BehaviorBuilderImpl : IBehaviorBuilder, ControlAccess.IBehaviorServicesController
{
    internal Action<IWin32State>? Win32Configurator { get; private set; }
    internal HandleRegistry? Registry { get; set; }

    internal void SetWin32Configurator(Action<IWin32State> configure) => Win32Configurator = configure;

    T ControlAccess.IBehaviorServicesController.Get<T>(string name)
        => (T)Registry!.GetByName(name)!;

    T ControlAccess.IBehaviorServicesController.GetWindow<T>(string name)
        => (T)Registry!.GetWindow(name)!;
}
