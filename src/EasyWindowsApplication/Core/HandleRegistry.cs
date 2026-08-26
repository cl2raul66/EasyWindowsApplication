using System.Collections.Concurrent;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal sealed class HandleRegistry
{
    private static readonly ConcurrentDictionary<nint, MasterRouter> _routersByHwnd = new();

    internal static void RegisterRouter(nint hwnd, MasterRouter router) => _routersByHwnd[hwnd] = router;
    internal static void UnregisterRouter(nint hwnd) => _routersByHwnd.TryRemove(hwnd, out _);
    internal static MasterRouter? GetRouter(nint hwnd) => _routersByHwnd.TryGetValue(hwnd, out var r) ? r : null;

    private readonly Dictionary<nint, IControl> _hwndToControl = new();
    private readonly Dictionary<string, IControl> _nameToControl = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IBaseWindow> _windowByName = new(StringComparer.Ordinal);

    internal void Register(nint hwnd, IControl control)
    {
        _hwndToControl[hwnd] = control;
        if (!string.IsNullOrEmpty(control.Name))
            _nameToControl[control.Name] = control;
    }

    internal void RegisterWindow(IBaseWindow window)
    {
        if (!string.IsNullOrEmpty(window.Name))
            _windowByName[window.Name] = window;
    }

    internal IControl? GetByHwnd(nint hwnd)
        => _hwndToControl.TryGetValue(hwnd, out var c) ? c : null;

    internal IControl? GetByName(string name)
        => _nameToControl.TryGetValue(name, out var c) ? c : null;

    internal T? GetByName<T>(string name) where T : class, IControl
        => GetByName(name) as T;

    internal T? GetWindow<T>(string name) where T : class, IBaseWindow
        => _windowByName.TryGetValue(name, out var w) ? w as T : null;

    internal IBaseWindow? GetWindow(string name)
        => _windowByName.TryGetValue(name, out var w) ? w : null;
}
