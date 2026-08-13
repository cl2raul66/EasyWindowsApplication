using System.Runtime.InteropServices;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.CoreModule.Backend;

internal sealed class MasterRouter
{
    private readonly Dictionary<(nint Hwnd, uint Msg), Win32MessageHandler> _handlers = new();
    private readonly HandleRegistry _registry;
    private nint _mainHwnd;

    internal MasterRouter(HandleRegistry registry)
    {
        _registry = registry;
    }

    internal void RegisterMainHwnd(nint hwnd) => _mainHwnd = hwnd;

    internal void RegisterHandler(nint hwnd, uint msg, Win32MessageHandler handler)
        => _handlers[(hwnd, msg)] = handler;

    internal void RemoveHandler(nint hwnd, uint msg)
        => _handlers.Remove((hwnd, msg));

    internal nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM.DESTROY && hwnd == _mainHwnd)
        {
            Win32.PostQuitMessage(0);
            return 0;
        }

        // 1. Try registered raw handlers (lowest level)
        if (hwnd == _mainHwnd && _handlers.TryGetValue((hwnd, msg), out var mainWindowHandler))
        {
            var result = mainWindowHandler(wParam, lParam);
            if (result != 0) return result;
        }

        nint controlHwnd = ResolveControlHwnd(msg, wParam, lParam);
        if (controlHwnd != 0 && _handlers.TryGetValue((controlHwnd, msg), out var controlHandler))
        {
            var result = controlHandler(wParam, lParam);
            if (result != 0) return result;
        }

        // 2. Dispatch typed events (high level)
        DispatchTypedEvent(msg, controlHwnd, wParam, lParam);

        return Win32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    private void DispatchTypedEvent(uint msg, nint controlHwnd, nint wParam, nint lParam)
    {
        if (controlHwnd == 0) return;

        var control = _registry.GetByHwnd(controlHwnd);
        if (control == null) return;

        switch (msg)
        {
            case WM.COMMAND:
                nint code = Win32Helpers.HIWORD(wParam);
                if ((code == BN.CLICKED || code == BN.DOUBLECLICKED) && control is IClickEventSource clickable)
                    clickable.RaiseClickInternal();
                break;
        }
    }

    private nint ResolveControlHwnd(uint msg, nint wParam, nint lParam)
    {
        return msg switch
        {
            WM.COMMAND => ResolveCommandHwnd(wParam, lParam),
            WM.NOTIFY => ResolveNotifyHwnd(lParam),
            _ => 0
        };
    }

    private nint ResolveCommandHwnd(nint wParam, nint lParam)
    {
        if (lParam != 0)
            return lParam;
        return 0;
    }

    private nint ResolveNotifyHwnd(nint lParam)
    {
        if (lParam == 0) return 0;
        var nmhdr = Marshal.PtrToStructure<NMHDR>(lParam);
        return nmhdr.hwndFrom;
    }
}
