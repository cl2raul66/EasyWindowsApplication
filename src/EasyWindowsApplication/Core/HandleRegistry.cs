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

    private readonly Dictionary<nint, WeakReference<IControl>> _hwndToControl = new();
    private readonly Dictionary<string, WeakReference<IControl>> _nameToControl = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IBaseWindow> _windowByName = new(StringComparer.Ordinal);
    private readonly Dictionary<nint, IBaseWindow> _hwndToWindow = new();
    private readonly Dictionary<nint, List<nint>> _windowToControls = new();

    internal void Register(nint hwnd, IControl control)
    {
        _hwndToControl[hwnd] = new WeakReference<IControl>(control);
        if (!string.IsNullOrEmpty(control.Name))
            _nameToControl[control.Name] = new WeakReference<IControl>(control);
    }

    internal void Unregister(nint hwnd)
    {
        if (_hwndToControl.TryGetValue(hwnd, out var weak))
        {
            if (weak.TryGetTarget(out var ctrl) && !string.IsNullOrEmpty(ctrl.Name))
            {
                // Solo elimina la entrada de nombre si apunta al mismo control
                if (_nameToControl.TryGetValue(ctrl.Name, out var nameWeak)
                    && nameWeak.TryGetTarget(out var nameCtrl)
                    && ReferenceEquals(nameCtrl, ctrl))
                {
                    _nameToControl.Remove(ctrl.Name);
                }
            }
            _hwndToControl.Remove(hwnd);
        }
    }

    internal void ClearAllControls()
    {
        _hwndToControl.Clear();
        _nameToControl.Clear();
        _windowToControls.Clear();
    }

    internal void TrackChildWindow(nint parentHwnd, nint childHwnd)
    {
        if (!_windowToControls.TryGetValue(parentHwnd, out var list))
        {
            list = new List<nint>();
            _windowToControls[parentHwnd] = list;
        }
        list.Add(childHwnd);
    }

    internal void UnregisterWindowControls(nint parentHwnd)
    {
        if (!_windowToControls.TryGetValue(parentHwnd, out var direct))
            return;
        var stack = new Stack<nint>(direct);
        _windowToControls.Remove(parentHwnd);
        while (stack.Count > 0)
        {
            var h = stack.Pop();
            Unregister(h);
            if (_windowToControls.TryGetValue(h, out var sub))
            {
                foreach (var ch in sub) stack.Push(ch);
                _windowToControls.Remove(h);
            }
        }
    }

    internal void RegisterWindow(IBaseWindow window)
    {
        if (!string.IsNullOrEmpty(window.Name))
            _windowByName[window.Name] = window;
        if (window.Hwnd != 0)
            _hwndToWindow[window.Hwnd] = window;
    }

    internal void UnregisterWindow(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            if (_windowByName.TryGetValue(name, out var w))
            {
                _windowByName.Remove(name);
                if (w.Hwnd != 0) _hwndToWindow.Remove(w.Hwnd);
            }
        }
    }

    internal void UnregisterWindowByHwnd(nint hwnd)
    {
        _hwndToWindow.Remove(hwnd);
        // también limpiar por nombre si existe
        string? keyToRemove = null;
        foreach (var kv in _windowByName)
            if (kv.Value.Hwnd == hwnd) { keyToRemove = kv.Key; break; }
        if (keyToRemove != null) _windowByName.Remove(keyToRemove);
    }

    internal IBaseWindow? GetWindowByHwnd(nint hwnd)
        => _hwndToWindow.TryGetValue(hwnd, out var w) ? w : null;

    internal T? GetWindowByHwnd<T>(nint hwnd) where T : class, IBaseWindow
        => GetWindowByHwnd(hwnd) as T;

    internal IControl? GetByHwnd(nint hwnd)
    {
        if (_hwndToControl.TryGetValue(hwnd, out var weak))
        {
            if (weak.TryGetTarget(out var c))
                return c;
            // Entrada muerta: limpieza perezosa
            _hwndToControl.Remove(hwnd);
        }
        return null;
    }

    internal IControl? GetByName(string name)
    {
        if (_nameToControl.TryGetValue(name, out var weak))
        {
            if (weak.TryGetTarget(out var c))
                return c;
            _nameToControl.Remove(name);
        }
        return null;
    }

    internal T? GetByName<T>(string name) where T : class, IControl
        => GetByName(name) as T;

    internal T? GetWindow<T>(string name) where T : class, IBaseWindow
        => _windowByName.TryGetValue(name, out var w) ? w as T : null;

    internal IBaseWindow? GetWindow(string name)
        => _windowByName.TryGetValue(name, out var w) ? w : null;
}
