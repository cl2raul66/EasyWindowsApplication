using System.Collections.Concurrent;
using System.Diagnostics;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal enum EntityKind : byte { Control, Window, Surface }

/// <summary>Política de baja de una ventana: hgwo muere (secundaria, re-creativa) o entidad muere (principal/teardown).</summary>
internal enum RegistryPolicy : byte { DetachHandle, Remove }

/// <summary>
/// Entidad registrada. Hwnd=0 para superficies sin HWND (menús, tray, diálogos)
/// o ventanas secundarias destruidas (DetachHandle conserva la identidad por Name).
/// </summary>
internal sealed class EntityEntry
{
    internal int Id;
    internal EntityKind Kind;
    internal nint Hwnd;
    internal string? Name;               // null = anónima (no accesible vía Behavior)
    internal object? Strong;             // Window / Surface (referencia fuerte)
    internal WeakReference<object>? Weak;        // Control (débil: el dueño es el modelo de layout)
    internal List<int>? ChildrenIds;     // solo ventanas (hijos por hwnd al Attach)

    internal object? Target =>
        Strong ?? (Weak is { } w && w.TryGetTarget(out var t) ? t : null);
}

/// <summary>
/// Almacén único de entidades con índices derivados.
/// Verdad: <see cref="_entries"/>. Proyecciones solo escritas por el motor:
/// <see cref="_byHwnd"/> (hwnd→Id) y <see cref="_byName"/> ((kind,name)→Id).
/// Ningún llamante escribe índices directamente — elimina la clase de bug #50
/// por construcción: DetachHandle (hwnd OS muere, identidad vive) vs Remove (identidad muere).
///
/// El trampolín estático <see cref="_routersByHwnd"/> queda fuera del modelo a propósito:
/// es plumbing del WndProc [UnmanagedCallersOnly]/GCHandle, no una entidad.
/// </summary>
internal sealed class HandleRegistry
{
    private static readonly ConcurrentDictionary<nint, MasterRouter> _routersByHwnd = new();

    internal static void RegisterRouter(nint hwnd, MasterRouter router) => _routersByHwnd[hwnd] = router;

    internal static void UnregisterRouter(nint hwnd)
    {
        _routersByHwnd.TryRemove(hwnd, out _);
        // Nota: el assert "router y registry no desincronizados" vive del lado del registry
        // (DebugAssertHwndUnregistered), llamada desde MasterRouter.WM_DESTROY justo antes.
    }

    /// <summary>
    /// Assert cross-check router↔registry para DEBUG: tras DetachWindowByHwnd + UnregisterRouter,
    /// el hwnd ya no debe existir en los índices de entidades.
    /// </summary>
    internal void DebugAssertHwndUnregistered(nint hwnd)
        => Debug.Assert(!_byHwnd.ContainsKey(hwnd),
            $"Registry leak: hwnd 0x{hwnd:X} sigue en _byHwnd tras detach/remove en WM_DESTROY.");
    internal static MasterRouter? GetRouter(nint hwnd) => _routersByHwnd.TryGetValue(hwnd, out var r) ? r : null;

    private int _nextId = 1;
    private readonly Dictionary<int, EntityEntry> _entries = [];
    private readonly Dictionary<nint, int> _byHwnd = [];
    private readonly Dictionary<(EntityKind Kind, string Name), int> _byName = [];

    // ── Motor ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Alta atómica: inserta Entry + proyecciones. parentHwnd enlaza jerarquía
    /// (ventana o control padre — el grafo sigue siendo hwnd-keyed como antes).
    /// </summary>
    internal int Attach(EntityKind kind, object target, nint hwnd = 0, string? name = null, nint parentHwnd = 0)
    {
        var entry = new EntityEntry { Id = _nextId++, Kind = kind, Hwnd = hwnd, Name = name };
        if (kind == EntityKind.Control)
            entry.Weak = new WeakReference<object>(target);
        else
            entry.Strong = target;

        _entries[entry.Id] = entry;
        if (hwnd != 0) _byHwnd[hwnd] = entry.Id;
        if (!string.IsNullOrEmpty(name)) _byName[(kind, name)] = entry.Id;

        if (parentHwnd != 0
            && _byHwnd.TryGetValue(parentHwnd, out var parentId)
            && _entries.TryGetValue(parentId, out var parent))
            (parent.ChildrenIds ??= []).Add(entry.Id);

        return entry.Id;
    }

    /// <summary>El HWND OS murió: borra la proyección hwnd; la identidad (Name) sobrevive.</summary>
    internal void DetachHandle(int id)
    {
        if (!_entries.TryGetValue(id, out var e)) return;
        if (e.Hwnd != 0) _byHwnd.Remove(e.Hwnd);
        e.Hwnd = 0;
    }

    /// <summary>La entidad deja de existir: sale de TODOS los índices, con cascada sobre hijos.</summary>
    internal void Remove(int id)
    {
        if (!_entries.Remove(id, out var e)) return;
        if (e.ChildrenIds is { } kids)
            foreach (var childId in kids) Remove(childId);
        if (e.Hwnd != 0) _byHwnd.Remove(e.Hwnd);
        if (!string.IsNullOrEmpty(e.Name)
            && _byName.TryGetValue((e.Kind, e.Name), out var existing)
            && existing == id)
            _byName.Remove((e.Kind, e.Name));
    }

    /// <summary>
    /// Dispatcher único para WM_DESTROY: cascada de controles (mueren siempre,
    /// se re-registran en re-creación) + política explícita sobre la ventana.
    /// </summary>
    internal void DetachWindowByHwnd(nint hwnd, RegistryPolicy policy)
    {
        if (!_byHwnd.TryGetValue(hwnd, out var id)
            || !_entries.TryGetValue(id, out var window)
            || window.Kind != EntityKind.Window)
            return;

        // Controles hijos: baja completa (hwnd + name). Re-Attach al re-crear.
        if (window.ChildrenIds is { } kids)
            foreach (var childId in kids) Remove(childId);
        window.ChildrenIds = null;

        if (policy == RegistryPolicy.Remove)
            Remove(id);
        else
            DetachHandle(id);   // secundaria: el nombre vive para el próximo Show()
    }

    /// <summary>Teardown determinístico: vacía toda la tabla (post message-loop).</summary>
    internal void RemoveAll()
    {
        var ids = new List<int>(_entries.Keys);
        foreach (var id in ids) Remove(id);
        Debug.Assert(_entries.Count == 0 && _byHwnd.Count == 0 && _byName.Count == 0,
            "HandleRegistry debe quedar vacía tras teardown (leak de entidad).");
    }

    // ── Lookups (fachada 1:1 sobre el motor) ─────────────────────────────────

    private EntityEntry? GetLive(int id)
    {
        // Limpieza perezosa centralizada de controles débiles muertos.
        if (!_entries.TryGetValue(id, out var e)) return null;
        if (e.Kind == EntityKind.Control && e.Target is null)
        {
            Remove(id);   // cascada segura: el control y sus hijos ya no existen
            return null;
        }
        return e;
    }

    internal IBaseWindow? GetWindowByHwnd(nint hwnd)
        => _byHwnd.TryGetValue(hwnd, out var id) && _entries.TryGetValue(id, out var e) && e.Kind == EntityKind.Window
            ? (IBaseWindow?)e.Target
            : null;

    internal T? GetWindowByHwnd<T>(nint hwnd) where T : class, IBaseWindow
        => GetWindowByHwnd(hwnd) as T;

    internal IControl? GetByHwnd(nint hwnd)
        => _byHwnd.TryGetValue(hwnd, out var id) ? GetLive(id)?.Target as IControl : null;

    internal IControl? GetByName(string name)
        => _byName.TryGetValue((EntityKind.Control, name), out var id)
            ? GetLive(id)?.Target as IControl
            : null;

    internal T? GetByName<T>(string name) where T : class, IControl
        => GetByName(name) as T;

    internal IBaseWindow? GetWindow(string name)
        => _byName.TryGetValue((EntityKind.Window, name), out var id) && _entries.TryGetValue(id, out var e)
            ? (IBaseWindow?)e.Target
            : null;

    internal T? GetWindow<T>(string name) where T : class, IBaseWindow
        => GetWindow(name) as T;

    internal IViewSurface? GetSurface(string name)
        => _byName.TryGetValue((EntityKind.Surface, name), out var id) && _entries.TryGetValue(id, out var e)
            ? (IViewSurface?)e.Target
            : null;

    internal T? GetSurface<T>(string name) where T : class, IViewSurface
        => GetSurface(name) as T;

    internal IEnumerable<nint> AllControlHandles()
    {
        foreach (var e in _entries.Values)
            if (e.Kind == EntityKind.Control && e.Hwnd != 0 && e.Target is not null)
                yield return e.Hwnd;
    }

    /// <summary>Instantánea de ventanas con HWND vivo (para teardown determinístico).</summary>
    internal List<IBaseWindow> LiveWindows()
    {
        var list = new List<IBaseWindow>();
        foreach (var e in _entries.Values)
            if (e.Kind == EntityKind.Window && e.Hwnd != 0 && e.Target is IBaseWindow w)
                list.Add(w);
        return list;
    }

    /// <summary>
    /// Alta (o re-alta tras re-creación) de ventana.
    /// Upsert por nombre: la identidad sobrevive ciclos Close()/Show() (#50).
    /// Solo el Hwnd cambia en cada ciclo.
    /// </summary>
    internal void RegisterWindow(IBaseWindow window)
    {
        if (!string.IsNullOrEmpty(window.Name)
            && _byName.TryGetValue((EntityKind.Window, window.Name), out var existingId)
            && _entries.TryGetValue(existingId, out var existing)
            && ReferenceEquals(existing.Target, window))
        {
            if (existing.Hwnd != 0) _byHwnd.Remove(existing.Hwnd);
            existing.Hwnd = window.Hwnd;
            if (window.Hwnd != 0) _byHwnd[window.Hwnd] = existing.Id;
            return;
        }
        Attach(EntityKind.Window, window, window.Hwnd, window.Name);
    }
}
