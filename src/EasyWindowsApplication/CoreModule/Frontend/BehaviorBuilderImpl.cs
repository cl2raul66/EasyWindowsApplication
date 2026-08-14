using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share.Infrastructure;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class BehaviorBuilderImpl : IBehaviorBuilder, ControlAccess.IBehaviorServicesController
{
    internal Action<IWin32State>? Win32Configurator { get; private set; }
    internal HandleRegistry? Registry { get; set; }
    private readonly List<(string Name, Action Handler)> _pendingClicks = new();

    public IBehaviorBuilder OnClick(string controlName, Action handler)
    {
        var control = Registry?.GetByName(controlName);
        if (control is IClickEventSource clickable)
        {
            clickable.AddClickHandler(handler);
        }
        else
        {
            _pendingClicks.Add((controlName, handler));
        }
        return this;
    }

    public IBehaviorBuilder WithWin32State(Action<IWin32State> configure)
    {
        Win32Configurator = configure;
        return this;
    }

    public T Get<T>(string name) where T : View<T>
    {
        var control = Registry?.GetByName(name);
        if (control is null)
            throw new InvalidOperationException($"Control '{name}' not found.");
        return (T)control;
    }

    T ControlAccess.IBehaviorServicesController.Get<T>(string name)
        => (T)Registry!.GetByName(name)!;

    T ControlAccess.IBehaviorServicesController.GetWindow<T>(string name)
        => (T)Registry!.GetWindow(name)!;

    internal void ApplyPending(HandleRegistry registry)
    {
        foreach (var (name, handler) in _pendingClicks)
        {
            if (registry.GetByName(name) is IClickEventSource clickable)
                clickable.AddClickHandler(handler);
        }
        _pendingClicks.Clear();
    }
}
