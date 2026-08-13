using System.Runtime.InteropServices;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Backend;

internal sealed class MasterRouter
{
    private readonly Dictionary<(nint Hwnd, uint Msg), Win32MessageHandler> _handlers = new();
    private readonly HandleRegistry _registry;
    private nint _mainHwnd;

    private readonly Dictionary<nint, nint> _windowBrushes = new();
    private readonly Dictionary<nint, nint> _controlBrushes = new();
    private readonly Dictionary<nint, List<(RECT Rect, int Color)>> _layoutGroupBackgrounds = new();
    private readonly Dictionary<nint, (int X, int Y)> _scrollOffsets = new();

    internal MasterRouter(HandleRegistry registry)
    {
        _registry = registry;
    }

    internal void AddLayoutGroupBackground(nint hwnd, RECT rect, Color color)
    {
        if (color.IsTransparent) return;
        int colorRef = color.ToCOLORREF();
        if (!_layoutGroupBackgrounds.TryGetValue(hwnd, out var list))
        {
            list = new List<(RECT, int)>();
            _layoutGroupBackgrounds[hwnd] = list;
        }
        list.Add((rect, colorRef));
    }

    internal void ClearLayoutGroupBackgrounds(nint hwnd)
        => _layoutGroupBackgrounds.Remove(hwnd);

    internal void RegisterMainHwnd(nint hwnd) => _mainHwnd = hwnd;

    internal void RegisterHandler(nint hwnd, uint msg, Win32MessageHandler handler)
        => _handlers[(hwnd, msg)] = handler;

    internal void RemoveHandler(nint hwnd, uint msg)
        => _handlers.Remove((hwnd, msg));

    internal void RegisterWindowBackgroundBrush(nint hwnd, nint brush)
    {
        if (brush != 0)
            _windowBrushes[hwnd] = brush;
    }

    internal void RegisterControlBrush(nint childHwnd, nint brush)
    {
        if (brush != 0)
            _controlBrushes[childHwnd] = brush;
    }

    internal void SetScrollOffset(nint hwnd, int x, int y)
    {
        _scrollOffsets[hwnd] = (x, y);
    }

    internal void ClearScrollOffset(nint hwnd)
    {
        _scrollOffsets.Remove(hwnd);
    }

    internal nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM.DESTROY && hwnd == _mainHwnd)
        {
            Win32.PostQuitMessage(0);
            return 0;
        }

        // Handle WM_ERASEBKGND — paint window background, then LayoutGroup backgrounds on top
        if (msg == WM.ERASEBKGND)
        {
            if (_windowBrushes.TryGetValue(hwnd, out var windowBrush))
            {
                Win32.GetClientRect(hwnd, out RECT rect);
                Win32.FillRect(wParam, ref rect, windowBrush);
            }

            if (_layoutGroupBackgrounds.TryGetValue(hwnd, out var backgrounds))
            {
                bool hasScroll = _scrollOffsets.TryGetValue(hwnd, out var scrollOff);
                foreach (var (bgRect, color) in backgrounds)
                {
                    var r = bgRect;
                    if (hasScroll)
                    {
                        r.Left -= scrollOff.X;
                        r.Top -= scrollOff.Y;
                        r.Right -= scrollOff.X;
                        r.Bottom -= scrollOff.Y;
                    }
                    nint brush = Win32.CreateSolidBrush(color);
                    Win32.FillRect(wParam, ref r, brush);
                    Win32.DeleteObject(brush);
                }
            }

            return 1;
        }

        // Handle WM_CTLCOLOR* — return control background brush
        if (msg >= WM.CTLCOLORMSGBOX && msg <= WM.CTLCOLORSTATIC)
        {
            nint childHwnd = lParam;
            if (_controlBrushes.TryGetValue(childHwnd, out var ctrlBrush))
                return ctrlBrush;
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
        if (control is null) return;

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
            WM.DRAWITEM => ResolveDrawItemHwnd(lParam),
            _ => 0
        };
    }

    private nint ResolveDrawItemHwnd(nint lParam)
    {
        if (lParam == 0) return 0;
        var dis = Marshal.PtrToStructure<DRAWITEMSTRUCT>(lParam);
        return dis.hwndItem;
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
