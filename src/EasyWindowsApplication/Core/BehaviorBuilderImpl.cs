using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.Core;

internal sealed class BehaviorBuilderImpl : IBehaviorBuilder, ControlAccess.IBehaviorServicesController, ControlAccess.ISurfaceServicesController
{
    private AppBehaviorBuilderImpl? _appBehavior;
    private ISystemTray? _systemTray;

    public IAppBehavior WindowsApplication => _appBehavior ??= new AppBehaviorBuilderImpl() { MainHwnd = MainHwnd };

    public ISystemTray SystemTray => _systemTray ??= (ISystemTray)Registry!.GetSurface("SystemTray")!;

    internal HandleRegistry? Registry { get; set; }

    internal nint MainHwnd { get; set; }

    internal void RaiseLaunched() => _appBehavior?.RaiseLaunched();

    internal bool RaiseTerminating() => _appBehavior?.RaiseTerminating() ?? false;

    internal void RaiseTerminated() => _appBehavior?.RaiseTerminated();

    T ControlAccess.IBehaviorServicesController.Get<T>(string name)
        => (T)Registry!.GetByName(name)!;

    T ControlAccess.IBehaviorServicesController.GetWindow<T>(string name)
        => (T)Registry!.GetWindow(name)!;

    T ControlAccess.ISurfaceServicesController.GetSurface<T>(string name)
        => (T)Registry!.GetSurface(name)!;
}
