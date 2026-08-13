using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class BehaviorBuilderImpl : IBehaviorBuilder
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

    public T Get<T>(string name) where T : ControlBase<T>
    {
        var control = Registry?.GetByName(name);
        if (control == null)
            throw new InvalidOperationException($"Control '{name}' not found.");
        return (T)control;
    }

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
