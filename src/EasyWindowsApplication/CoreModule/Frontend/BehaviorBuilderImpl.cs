using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class BehaviorBuilderImpl : IBehaviorBuilder, ControlAccess.IBehaviorServicesController
{
    internal HandleRegistry? Registry { get; set; }

    T ControlAccess.IBehaviorServicesController.Get<T>(string name)
        => (T)Registry!.GetByName(name)!;

    T ControlAccess.IBehaviorServicesController.GetWindow<T>(string name)
        => (T)Registry!.GetWindow(name)!;
}
