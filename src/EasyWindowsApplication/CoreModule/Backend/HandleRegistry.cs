using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.CoreModule.Backend;

internal sealed class HandleRegistry
{
    private readonly Dictionary<nint, IControl> _hwndToControl = new();
    private readonly Dictionary<string, IControl> _nameToControl = new(StringComparer.Ordinal);

    internal void Register(nint hwnd, IControl control)
    {
        _hwndToControl[hwnd] = control;
        if (!string.IsNullOrEmpty(control.Name))
            _nameToControl[control.Name] = control;
    }

    internal IControl? GetByHwnd(nint hwnd)
        => _hwndToControl.TryGetValue(hwnd, out var c) ? c : null;

    internal IControl? GetByName(string name)
        => _nameToControl.TryGetValue(name, out var c) ? c : null;

    internal T? GetByName<T>(string name) where T : class, IControl
        => GetByName(name) as T;
}
