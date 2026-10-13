namespace EasyWindowsApplication.Core.Surfaces;

/// <summary>
/// HWND del control que disparó el trigger en curso (hilo UI).
/// Permite a <c>MenuSurface.Show()</c> anclarse al control sin cambiar
/// las firmas <c>Action</c> de <c>IInputSurface</c>. Solo vive durante
/// el dispatch síncrono (<c>Fire</c> → handlers); se limpia con
/// <c>try/finally</c> en cada punto de publicación.
/// </summary>
internal static class InputTriggerContext
{
    private static readonly AsyncLocal<(nint Hwnd, int Tick)> _current = new();

    internal static void Publish(nint hwnd)
    {
        if (hwnd == 0) return;
        _current.Value = (hwnd, Environment.TickCount);
    }

    internal static void Clear() => _current.Value = default;

    internal static bool TryGetFresh(out nint hwnd, int maxAgeMs = 2000)
    {
        var (h, tick) = _current.Value;
        hwnd = h;
        if (h == 0) return false;
        return unchecked(Environment.TickCount - tick) <= maxAgeMs;
    }
}
