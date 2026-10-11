using System.Runtime.InteropServices;
using EasyWindowsApplication.Core.Surfaces;
using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.SystemTray;

internal sealed class SystemTrayBroker
{
    internal const nuint HoldTimerId = 1;
    internal const uint HoldThresholdMs = 500;
    internal const uint HoverResetMs = 1000;
    internal const uint IconId = 1;

    private const uint WM_MOUSEMOVE = 0x0200;
    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_MBUTTONUP = 0x0208;

    private readonly Action<Type, int> _dispatch;
    private readonly uint _callbackMessage;

    private bool _iconAdded;
    private string _tooltip = "";
    private nint _icon;
    private bool _disposed;

    private int _lastHoverTick;
    private bool _holding;
    private readonly TapChainCounter _counter = new();

    internal nint Hwnd { get; }
    internal uint CallbackMessage => _callbackMessage;

    private SystemTrayBroker(nint hwnd, uint callbackMessage, Action<Type, int> dispatch)
    {
        Hwnd = hwnd;
        _callbackMessage = callbackMessage;
        _dispatch = dispatch;
    }

    internal static SystemTrayBroker Create(MasterRouter router, Action<Type, int> dispatch)
    {
        nint hwnd = Windowing.Procedures.CreateHiddenWindow(router);
        uint callback = Win32.RegisterWindowMessageW("EasyWindowsApplication.Tray.v1");
        if (callback == 0)
            throw new InvalidOperationException("RegisterWindowMessageW failed for tray callback.");
        var broker = new SystemTrayBroker(hwnd, callback, dispatch);
        router.RegisterHandler(hwnd, callback, broker.OnCallback);
        router.RegisterHandler(hwnd, WM.CONTEXTMENU, broker.OnContextMenu);
        router.RegisterHandler(hwnd, WM.TIMER, broker.OnTimer);
        router.RegisterHandler(hwnd, WM.DESTROY, broker.OnDestroy);
        return broker;
    }

    internal bool AddIcon(nint icon)
    {
        _icon = icon;
        var data = NewData(NIF.MESSAGE | NIF.ICON | NIF.TIP | NIF.SHOWTIP);
        CopyStrings(ref data, _tooltip, null, null);
        if (!Win32.Shell_NotifyIconW(NIM.ADD, ref data)) return false;
        _iconAdded = true;
        data.uVersion = NOTIFYICON.VERSION_4;
        Win32.Shell_NotifyIconW(NIM.SETVERSION, ref data);
        return true;
    }

    internal void RemoveIcon()
    {
        if (!_iconAdded) return;
        var data = NewData(0);
        Win32.Shell_NotifyIconW(NIM.DELETE, ref data);
        _iconAdded = false;
    }

    internal void UpdateTooltip(string tooltip)
    {
        _tooltip = tooltip;
        if (!_iconAdded) return;
        var data = NewData(NIF.TIP | NIF.SHOWTIP);
        CopyStrings(ref data, tooltip, null, null);
        Win32.Shell_NotifyIconW(NIM.MODIFY, ref data);
    }

    internal void ShowBalloon(string title, string text)
    {
        if (!_iconAdded) return;
        var data = NewData(NIF.INFO);
        CopyStrings(ref data, null, text, title);
        Win32.Shell_NotifyIconW(NIM.MODIFY, ref data);
    }

    internal void HideBalloon()
    {
        if (!_iconAdded) return;
        var data = NewData(NIF.INFO);
        Win32.Shell_NotifyIconW(NIM.MODIFY, ref data);
    }

    internal void Shutdown()
    {
        if (_disposed) return;
        _disposed = true;
        Win32.KillTimer(Hwnd, HoldTimerId);
        RemoveIcon();
        Windowing.Win32.DestroyWindow(Hwnd);
        HandleRegistry.UnregisterRouter(Hwnd);
    }

    internal void SetVisible(bool visible)
    {
        if (_disposed) return;
        if (visible)
        {
            if (!_iconAdded) AddIcon(_icon);
        }
        else
        {
            RemoveIcon();
        }
    }

    private nint OnCallback(nint wParam, nint lParam)
    {
        // VERSION_4 empaqueta lParam = mensaje (LOWORD) | uID del icono (HIWORD).
        uint packed = (uint)lParam;
        if ((packed >> 16) != IconId) return 0;
        switch (packed & 0xFFFF)
        {
            case WM_MOUSEMOVE: OnHover(); break;
            case WM_LBUTTONDOWN: OnButtonDown(); break;
            case WM_LBUTTONUP: OnButtonUp(); break;
            // WM_LBUTTONDBLCLK se ignora a propósito: el doble-tap ya se codifica
            // como cadena MainTap + TwoTap en el segundo WM_LBUTTONUP (§4.2).
            case WM_RBUTTONUP: if (IsCursorOverIcon()) Fire(typeof(AlternativeTap1)); break;
            case WM_MBUTTONUP: if (IsCursorOverIcon()) Fire(typeof(AlternativeTap2)); break;
            case WM.CONTEXTMENU: OnContextMenuKey(); break;
            case var m when m == NIN.SELECT || m == NIN.KEYSELECT: Fire(typeof(MainTap)); break;
            case var m when m == NIN.POPUPOPEN: OnHover(); break;
        }
        return 0;
    }

    private nint OnContextMenu(nint wParam, nint lParam)
    {
        if (lParam != -1) return 0;
        Fire(typeof(KeyMenu));
        if (Win32.GetKeyState(VK.SHIFT) < 0)
            _dispatch(typeof(Chord<KeyShift, KeyF10>), 1);
        return 0;
    }

    private void OnContextMenuKey()
    {
        // WM_CONTEXTMENU por callback llega igual con ratón y con teclado.
        // Heurística: el click de ratón exige el cursor sobre el icono;
        // la tecla de menú funciona con el cursor en cualquier parte.
        if (IsCursorOverIcon())
        {
            Fire(typeof(AlternativeTap1));
            return;
        }
        Fire(typeof(KeyMenu));
        if (Win32.GetKeyState(VK.SHIFT) < 0)
            _dispatch(typeof(Chord<KeyShift, KeyF10>), 1);
    }

    private bool IsCursorOverIcon()
    {
        if (!Win32.GetCursorPos(out var pt)) return false;
        var identifier = new NOTIFYICONIDENTIFIER
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONIDENTIFIER>(),
            hWnd = Hwnd,
            uID = IconId,
            guidItem = Guid.Empty
        };
        if (Win32.Shell_NotifyIconGetRect(ref identifier, out var rect) != 0)
            return false;
        return pt.X >= rect.Left && pt.X < rect.Right && pt.Y >= rect.Top && pt.Y < rect.Bottom;
    }

    private nint OnTimer(nint wParam, nint lParam)
    {
        if ((nuint)wParam != HoldTimerId) return 0;
        Win32.KillTimer(Hwnd, HoldTimerId);
        _holding = true;
        Fire(typeof(Holding));
        return 0;
    }

    private nint OnDestroy(nint wParam, nint lParam)
    {
        Win32.KillTimer(Hwnd, HoldTimerId);
        RemoveIcon();
        HandleRegistry.UnregisterRouter(Hwnd);
        return 0;
    }

    private void OnButtonDown()
    {
        _holding = false;
        Win32.SetTimer(Hwnd, HoldTimerId, HoldThresholdMs, 0);
    }

    private void OnButtonUp()
    {
        Win32.KillTimer(Hwnd, HoldTimerId);
        if (_holding)
        {
            _holding = false;
            Fire(typeof(LongTap));
        }
        else
        {
            Fire(typeof(MainTap));
        }
    }

    private void OnHover()
    {
        int now = Environment.TickCount;
        if (_lastHoverTick != 0 && unchecked(now - _lastHoverTick) < (int)HoverResetMs) return;
        _lastHoverTick = now;
        Fire(typeof(Hover));
    }

    private void Fire(Type trigger)
    {
        int count = _counter.Next(trigger, Environment.TickCount, Win32.GetDoubleClickTime());
        _dispatch(trigger, count);
    }

    private NOTIFYICONDATAW NewData(uint flags)
    {
        return new NOTIFYICONDATAW
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>(),
            hWnd = Hwnd,
            uID = IconId,
            uFlags = flags,
            uCallbackMessage = _callbackMessage,
            hIcon = _icon
        };
    }

    private static unsafe void CopyStrings(ref NOTIFYICONDATAW data, string? tip, string? info, string? infoTitle)
    {
        fixed (NOTIFYICONDATAW* p = &data)
        {
            if (tip is not null) CopyText(p->szTip, 128, tip);
            if (info is not null) CopyText(p->szInfo, 256, info);
            if (infoTitle is not null) CopyText(p->szInfoTitle, 64, infoTitle);
        }
    }

    private static unsafe void CopyText(char* dst, int capacity, string text)
    {
        int n = Math.Min(text.Length, capacity - 1);
        for (int i = 0; i < n; i++) dst[i] = text[i];
        dst[n] = '\0';
    }
}
