using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class LayoutEngine
{
    private readonly ILayoutStrategy _strategy;

    public LayoutEngine(ILayoutStrategy strategy)
    {
        _strategy = strategy;
    }

    public void Execute(
        IReadOnlyList<ILayoutable> children,
        float availableWidth,
        float availableHeight,
        float spacing,
        Thickness padding)
    {
        if (children.Count == 0) return;

        _strategy.Measure(children, availableWidth, availableHeight);
        _strategy.Arrange(children, availableWidth, availableHeight, spacing, padding);

        // Batching Win32: cero flickering (DeferWindowPos)
        nint hdwp = Win32.BeginDeferWindowPos(children.Count);
        bool batching = hdwp != 0;

        if (batching)
        {
            foreach (var child in children)
            {
                if (child is ControlBase cb && cb.Hwnd != 0)
                {
                    nint next = Win32.DeferWindowPos(hdwp, cb.Hwnd, 0,
                        (int)cb._arrangedX, (int)cb._arrangedY, (int)cb._arrangedW, (int)cb._arrangedH,
                        SWP.NOZORDER | SWP.NOACTIVATE);
                    if (next != 0) hdwp = next;
                    else batching = false; // fallback a Render individual si falla
                }
                else
                {
                    // No es ControlBase (ej. contenedor lógico) → Render directo
                    child.Render();
                }
            }

            if (batching)
            {
                Win32.EndDeferWindowPos(hdwp);
                return;
            }
            // Si falló batching, EndDefer y fallback
            if (hdwp != 0) Win32.EndDeferWindowPos(hdwp);
        }

        // Fallback sin batching
        foreach (var child in children)
            child.Render();
    }
}
