using System.Runtime.InteropServices;
using EasyWindowsApplication.Core.Windowing;
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
    private const uint WM_LBUTTONDBLCLK = 0x0203;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_MBUTTONUP = 0x0208;

    private readonly Action<Type, int> _dispatch;
    private readonly uint _callbackMessage;

    private bool _iconAdded;
    private string _tooltip = "";
    private nint _icon;
    private bool _disposed;

    private Type? _lastTrigger;
    private int _lastCount;
    private int _lastTick;
    private int _lastHoverTick;
    private bool _holding;

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
        var data = NewData(NIF.MESSAGE | NIF.ICON | NIF.TIP);
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
        var data = NewData(NIF.TIP);
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
        switch ((uint)lParam)
        {
            case WM_MOUSEMOVE: OnHover(); break;
            case WM_LBUTTONDOWN: OnButtonDown(); break;
            case WM_LBUTTONUP: OnButtonUp(); break;
            case WM_LBUTTONDBLCLK: Fire(typeof(MainDoubleTap)); break;
            case WM_RBUTTONUP: Fire(typeof(AlternativeTap1)); break;
            case WM_MBUTTONUP: Fire(typeof(AlternativeTap2)); break;
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
        int count = 1;
        if (IsChainable(trigger) && trigger == _lastTrigger)
        {
            int now = Environment.TickCount;
            if (unchecked(now - _lastTick) <= (int)Win32.GetDoubleClickTime())
                count = Math.Min(_lastCount + 1, 10);
        }
        if (IsChainable(trigger))
        {
            _lastTrigger = trigger;
            _lastCount = count;
            _lastTick = Environment.TickCount;
        }
        else
        {
            _lastTrigger = null;
            _lastCount = 0;
        }
        _dispatch(trigger, count);
    }

    private static bool IsChainable(Type trigger)
        => trigger == typeof(MainTap)
            || trigger == typeof(AlternativeTap1)
            || trigger == typeof(AlternativeTap2)
            || trigger == typeof(LongTap);

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
