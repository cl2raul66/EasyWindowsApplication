using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core.Windowing;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Core;

internal sealed class MasterRouter
{
    private readonly Dictionary<(nint Hwnd, uint Msg), Win32MessageHandler> _handlers = new();
    private readonly HandleRegistry _registry;
    private nint _mainHwnd;
    private bool _isResizing;

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

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    internal static nint WndProcTrampoline(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        try
        {
            var router = HandleRegistry.GetRouter(hwnd);
            return router?.WndProc(hwnd, msg, wParam, lParam) ?? Win32.DefWindowProcW(hwnd, msg, wParam, lParam);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Fail(ex.ToString());
            return Win32.DefWindowProcW(hwnd, msg, wParam, lParam);
        }
    }

    internal nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        // ── Window lifecycle & resize routing (Fase 3) ──
        if (msg == WM.CLOSE)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                bool cancel = false;
                if (win is WindowImpl wi) cancel = wi.RaiseClosing();
                else if (win is AlternativeWindowImpl aw) cancel = aw.RaiseClosing();
                if (cancel) return 0;
            }
        }

        if (msg == WM.ENTERSIZEMOVE)
        {
            _isResizing = true;
        }
        else if (msg == WM.EXITSIZEMOVE)
        {
            _isResizing = false;
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                Win32.GetClientRect(hwnd, out RECT rc);
                int w = rc.Right - rc.Left;
                int h = rc.Bottom - rc.Top;
                if (win is WindowImpl wi) wi.RaiseResized(w, h);
            }
        }
        else if (msg == WM.SIZE)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                int wParamType = (int)wParam;
                if (wParamType != WMSIZE.MINIMIZED)
                {
                    int w = (int)Win32Helpers.LOWORD(lParam);
                    int h = (int)Win32Helpers.HIWORD(lParam);
                    if (_isResizing)
                    {
                        if (win is WindowImpl wi) wi.RaiseResizing(w, h);
                    }
                    else
                    {
                        if (win is WindowImpl wi) wi.RaiseResized(w, h);
                    }
                }
            }
        }
        else if (msg == WM.MOVE)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                int x = (int)(short)Win32Helpers.LOWORD(lParam);
                int y = (int)(short)Win32Helpers.HIWORD(lParam);
                if (win is WindowImpl wi) wi.RaiseMoved(x, y);
            }
        }
        else if (msg == WM.ACTIVATE)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                int state = (int)Win32Helpers.LOWORD(wParam);
                if (state == WA.INACTIVE)
                {
                    if (win is WindowImpl wi) wi.RaiseDeactivated();
                    else if (win is AlternativeWindowImpl aw) aw.RaiseDeactivated();
                }
                else
                {
                    if (win is WindowImpl wi) wi.RaiseActivated();
                    else if (win is AlternativeWindowImpl aw) aw.RaiseActivated();
                }
            }
        }
        else if (msg == WM.HSCROLL)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win is WindowImpl wi)
            {
                int request = (int)Win32Helpers.LOWORD(wParam);
                int pos = (int)Win32Helpers.HIWORD(wParam);
                wi.HandleHScroll(request, pos);
                return 0;
            }
        }
        else if (msg == WM.VSCROLL)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win is WindowImpl wi)
            {
                int request = (int)Win32Helpers.LOWORD(wParam);
                int pos = (int)Win32Helpers.HIWORD(wParam);
                wi.HandleVScroll(request, pos);
                return 0;
            }
        }
        else if (msg == WM.MOUSEWHEEL)
        {
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win is WindowImpl wi)
            {
                short delta = (short)Win32Helpers.HIWORD(wParam);
                wi.HandleMouseWheel(delta);
                return 0;
            }
        }
        else if (msg == WM.SETTINGCHANGE)
        {
            // SPI_SETNONCLIENTMETRICS sent when system font/theme changes
            if ((uint)wParam == SPI.SETNONCLIENTMETRICS)
            {
                try { Win32ControlsModule.Backend.ControlProcedures.InvalidateDefaultFont(); } catch { }
                nint newFont = 0;
                try { newFont = Win32ControlsModule.Backend.ControlProcedures.GetDefaultFont(); } catch { }
                if (newFont != 0)
                {
                    foreach (var ch in _registry.AllControlHandles())
                    {
                        try { Win32.SendMessageW(ch, WM.SETFONT, newFont, 1); } catch { }
                    }
                }
            }
        }
        else if (msg == WM.DPICHANGED)
        {
            try { Win32ControlsModule.Backend.ControlProcedures.InvalidateDefaultFont(); } catch { }
            nint newFont = 0;
            try { newFont = Win32ControlsModule.Backend.ControlProcedures.GetDefaultFont(); } catch { }
            if (newFont != 0)
            {
                foreach (var ch in _registry.AllControlHandles())
                {
                    try { Win32.SendMessageW(ch, WM.SETFONT, newFont, 1); } catch { }
                }
            }
        }

        if (msg == WM.DESTROY)
        {
            // Disparar Closed antes de limpieza
            var win = _registry.GetWindowByHwnd(hwnd);
            if (win != null)
            {
                if (win is WindowImpl wi) wi.RaiseClosed();
                else if (win is AlternativeWindowImpl aw) aw.RaiseClosed();
            }

            // Limpieza determinística del registry (Fase 5: evita memory leak de controles)
            _registry.Unregister(hwnd);
            _registry.UnregisterWindowControls(hwnd);
            _registry.UnregisterWindowByHwnd(hwnd);
            HandleRegistry.UnregisterRouter(hwnd);
            _windowBrushes.Remove(hwnd);
            _controlBrushes.Remove(hwnd);
            _layoutGroupBackgrounds.Remove(hwnd);
            _scrollOffsets.Remove(hwnd);
            // Limpia handlers asociados a este hwnd (evita leak de delegates)
            if (_handlers.Count > 0)
            {
                // Copia claves para evitar modificar durante enumeración
                var toRemove = new List<(nint Hwnd, uint Msg)>();
                foreach (var key in _handlers.Keys)
                    if (key.Hwnd == hwnd) toRemove.Add(key);
                foreach (var key in toRemove) _handlers.Remove(key);
            }
            if (hwnd == _mainHwnd)
            {
                Win32.PostQuitMessage(0);
                return 0;
            }
        }

        // Handle WM_ERASEBKGND — paint window background, then LayoutGroup backgrounds on top
        if (msg == WM.ERASEBKGND)
        {
            bool painted = false;

            if (_windowBrushes.TryGetValue(hwnd, out var windowBrush))
            {
                Win32.GetClientRect(hwnd, out RECT rect);
                Win32.FillRect(wParam, ref rect, windowBrush);
                painted = true;
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
                painted = true;
            }

            if (painted) return 1;
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
