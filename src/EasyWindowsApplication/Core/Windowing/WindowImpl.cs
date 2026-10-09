using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core.LayoutEngine;
using EasyWindowsApplication.Core.Surfaces;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Core.Windowing;

internal sealed class WindowImpl : IWindow, IInputFeed
{
    private readonly SurfaceInputHub _hub = new();
    void IInputFeed.FeedTrigger(Type trigger, int count) => _hub.Fire(trigger, count);

    public void OnInputWithSpatialPosition<TTrigger>(Action handler) where TTrigger : ISpatialPositionTrigger
        => OnInputWithSpatialPosition<TTrigger, AnyTap>(handler);

    public void OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCountSpec
        => _hub.AddSpatial(typeof(TTrigger), typeof(TCount), handler);

    public void OnInputWithSpatialPosition(Action handler)
        => OnInputWithSpatialPosition<MainTap, OneTap>(handler);

    public void OnInputWithoutSpatialPosition<TTrigger>(Action handler) where TTrigger : WithoutSpatialPositionTrigger
        => _hub.AddNonSpatial(typeof(TTrigger), handler);

    public nint Hwnd { get; }
    public string Name { get; }
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value ?? "";
            if (Hwnd != 0) Core.Win32.SetWindowText(Hwnd, _title);
        }
    }

    private float _width;
    public float Width
    {
        get => _width;
        set
        {
            if (_width == value) return;
            _width = value;
            if (Hwnd != 0)
                Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, (int)_width, (int)_height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
        }
    }

    private float _height;
    public float Height
    {
        get => _height;
        set
        {
            if (_height == value) return;
            _height = value;
            if (Hwnd != 0)
                Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, (int)_width, (int)_height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
        }
    }

    private WindowPositionOnScreen _positionMode;
    public WindowPositionOnScreen PositionMode
    {
        get => _positionMode;
        set
        {
            if (_positionMode == value) return;
            _positionMode = value;
            if (Hwnd != 0 && _positionMode == WindowPositionOnScreen.Center)
                Center();
        }
    }

    private List<ILayoutable>? _materializedChildren;
    private ContentModel? _contentModel;

    // Scroll state (Fase 4)
    private WindowsScroll? _scrollConfig;
    private int _scrollX;
    private int _scrollY;
    private int _maxScrollX;
    private int _maxScrollY;
    private int _clientW;
    private int _clientH;

    public (int X, int Y) ScrollOffset => (_scrollX, _scrollY);

    public event EventHandler? Loaded;
    public event EventHandler? Activated;
    public event EventHandler? Deactivated;
    public event EventHandler<WindowResizingEventArgs>? Resizing;
    public event EventHandler<WindowResizedEventArgs>? Resized;
    public event EventHandler<WindowMovedEventArgs>? Moved;

    // Behavior-first lifecycle & geometry
    public void OnLoaded(Action handler) => Loaded += (_, _) => handler();
    public void OnActivated(Action handler) => Activated += (_, _) => handler();
    public void OnDeactivated(Action handler) => Deactivated += (_, _) => handler();
    public void OnResizing(Action<WindowResizingEventArgs> handler) => Resizing += (_, e) => handler(e);
    public void OnResized(Action<WindowResizedEventArgs> handler) => Resized += (_, e) => handler(e);
    public void OnMoved(Action<WindowMovedEventArgs> handler) => Moved += (_, e) => handler(e);

    internal WindowImpl(nint hwnd, string name, string title, float width, float height, WindowPositionOnScreen position)
    {
        Hwnd = hwnd;
        Name = name;
        _title = title ?? "";
        _width = width;
        _height = height;
        _positionMode = position;
    }

    public void Show() => Win32.ShowWindow(Hwnd, SW.SHOW);
    public void Hide() => Win32.ShowWindow(Hwnd, SW.HIDE);
    public void Visibility(bool visible)
    {
        if (visible) Show();
        else Hide();
    }

    public void Center()
    {
        if (Hwnd == 0) return;
        nint monitor = Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
        if (monitor == 0) monitor = Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTOPRIMARY);
        if (monitor == 0) return;

        MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
        if (!Core.Win32.GetMonitorInfoW(monitor, ref mi)) return;

        Core.Win32.GetWindowRect(Hwnd, out RECT wr);
        float winW = wr.Right - wr.Left;
        float winH = wr.Bottom - wr.Top;

        float workW = mi.rcWork.Right - mi.rcWork.Left;
        float workH = mi.rcWork.Bottom - mi.rcWork.Top;

        int x = (int)(mi.rcWork.Left + (workW - winW) / 2);
        int y = (int)(mi.rcWork.Top + (workH - winH) / 2);

        Core.Win32.SetWindowPos(Hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
    }

    public void Maximize() => Core.Win32.ShowWindow(Hwnd, SW.MAXIMIZE);
    public void Minimize() => Core.Win32.ShowWindow(Hwnd, SW.MINIMIZE);
    public void Restore() => Core.Win32.ShowWindow(Hwnd, SW.RESTORE);
    public void Focus() => Core.Win32.SetForegroundWindow(Hwnd);

    // ── Phase C — #47 ────────────────────────────────────────────────────────

    private WindowState _state = WindowState.Normal;
    public WindowState State
    {
        get => Core.Win32.IsZoomed(Hwnd) ? WindowState.Maximized
             : Core.Win32.IsIconic(Hwnd) ? WindowState.Minimized
             : _state == WindowState.FullScreen ? WindowState.FullScreen
             : WindowState.Normal;
        set
        {
            _state = value;
            switch (value)
            {
                case WindowState.Normal: Restore(); break;
                case WindowState.Minimized: Minimize(); break;
                case WindowState.Maximized: Maximize(); break;
                case WindowState.FullScreen: EnterFullScreen(); break;
            }
        }
    }

    // FullScreen nativo: guarda estilos y rect original; restaura en ExitFullScreen.
    private int _fsOrigStyle;
    private int _fsOrigExStyle;
    private RECT _fsOrigRect;
    private bool _isFullScreen;

    public void EnterFullScreen()
    {
        if (Hwnd == 0 || _isFullScreen) return;
        _isFullScreen = true;
        _fsOrigStyle = Core.Win32.GetWindowLongW(Hwnd, GWL.STYLE);
        _fsOrigExStyle = Core.Win32.GetWindowLongW(Hwnd, GWL.EXSTYLE);
        Core.Win32.GetWindowRect(Hwnd, out _fsOrigRect);
        int newStyle = _fsOrigStyle & ~(int)(WS.CAPTION | WS.THICKFRAME);
        Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, newStyle);
        nint mon = Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
        if (mon != 0)
        {
            MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
            if (Core.Win32.GetMonitorInfoW(mon, ref mi))
                Core.Win32.SetWindowPos(Hwnd, 0, mi.rcMonitor.Left, mi.rcMonitor.Top,
                    mi.rcMonitor.Right - mi.rcMonitor.Left, mi.rcMonitor.Bottom - mi.rcMonitor.Top,
                    SWP.NOZORDER | SWP.NOACTIVATE | SWP.FRAMECHANGED);
        }
        _state = WindowState.FullScreen;
    }

    public void ExitFullScreen()
    {
        if (Hwnd == 0 || !_isFullScreen) return;
        _isFullScreen = false;
        Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, _fsOrigStyle);
        Core.Win32.SetWindowLongW(Hwnd, GWL.EXSTYLE, _fsOrigExStyle);
        int w = _fsOrigRect.Right - _fsOrigRect.Left;
        int h = _fsOrigRect.Bottom - _fsOrigRect.Top;
        Core.Win32.SetWindowPos(Hwnd, 0, _fsOrigRect.Left, _fsOrigRect.Top, w, h,
            SWP.NOZORDER | SWP.NOACTIVATE | SWP.FRAMECHANGED);
        _state = WindowState.Normal;
    }

    private bool _topMost;
    public bool TopMost
    {
        get => _topMost;
        set
        {
            if (_topMost == value) return;
            _topMost = value;
            // HWND_TOPMOST = -1; HWND_NOTOPMOST = -2
            Core.Win32.SetWindowPos(Hwnd, value ? -1 : -2, 0, 0, 0, 0,
                SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE);
        }
    }

    private double _opacity = 1.0;
    public double Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0.0, 1.0);
            if (Hwnd == 0) return;
            if (_opacity < 1.0)
            {
                int ex = Core.Win32.GetWindowLongW(Hwnd, GWL.EXSTYLE);
                if ((ex & (int)WS_EX.LAYERED) == 0)
                    Core.Win32.SetWindowLongW(Hwnd, GWL.EXSTYLE, ex | (int)WS_EX.LAYERED);
                Core.Win32.SetLayeredWindowAttributes(Hwnd, 0, (byte)Math.Round(_opacity * 255), LWA.ALPHA);
            }
            else
            {
                int ex = Core.Win32.GetWindowLongW(Hwnd, GWL.EXSTYLE);
                if ((ex & (int)WS_EX.LAYERED) != 0)
                    Core.Win32.SetWindowLongW(Hwnd, GWL.EXSTYLE, ex & ~(int)WS_EX.LAYERED);
                Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0,
                    SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE | SWP.FRAMECHANGED);
            }
        }
    }

    private WindowBackdrop _backdrop = WindowBackdrop.None;
    public WindowBackdrop Backdrop
    {
        get => _backdrop;
        set
        {
            if (_backdrop == value) return;
            _backdrop = value;
            if (Hwnd == 0 || !Core.Win32.IsWindows10_1809OrLater()) return;
            int type = value switch
            {
                WindowBackdrop.Mica => DWMSBT.MAINWINDOW,
                WindowBackdrop.Acrylic or WindowBackdrop.DesktopAcrylic => DWMSBT.TRANSIENTWINDOW,
                WindowBackdrop.SolidColor => DWMSBT.NONE,
                _ => DWMSBT.AUTO,
            };
            int hr = Core.Win32.DwmSetWindowAttribute(Hwnd, DWMWA.SYSTEMBACKDROP_TYPE, ref type, sizeof(int));
            System.Diagnostics.Debug.WriteLineIf(hr != 0, $"[EWA] DwmSetWindowAttribute(SYSTEMBACKDROP_TYPE={type}) falló: hr=0x{hr:X8}");
        }
    }

    private ScreenCaptureMode _screenCapture = ScreenCaptureMode.Allow;
    public ScreenCaptureMode ScreenCapture
    {
        get => _screenCapture;
        set
        {
            if (_screenCapture == value) return;
            _screenCapture = value;
            if (Hwnd == 0) return;
            uint affinity = value switch
            {
                ScreenCaptureMode.HideContent or ScreenCaptureMode.HideWindow => WDA.EXCLUDEFROMCAPTURE,
                _ => WDA.NONE,
            };
            if (!Core.Win32.SetWindowDisplayAffinity(Hwnd, affinity) && affinity == WDA.EXCLUDEFROMCAPTURE)
                Core.Win32.SetWindowDisplayAffinity(Hwnd, WDA.MONITOR); // fallback Win10 < 2004
        }
    }

    // Phase B: sizing constraints (el router los aplica en WM_GETMINMAXINFO)
    private (int W, int H)? _minSize;
    public (int MinWidth, int MinHeight) MinSize
    {
        get => _minSize ?? (0, 0);
        set => _minSize = value;
    }

    private (int W, int H)? _maxSize;
    public (int MaxWidth, int MaxHeight) MaxSize
    {
        get => _maxSize ?? (0, 0);
        set => _maxSize = value;
    }

    private bool _resizable = true;
    public bool Resizable
    {
        get => _resizable;
        set
        {
            if (_resizable == value) return;
            _resizable = value;
            if (Hwnd == 0) return;
            int style = Core.Win32.GetWindowLongW(Hwnd, GWL.STYLE);
            int newStyle = value ? style | (int)WS.THICKFRAME : style & ~(int)WS.THICKFRAME;
            if (newStyle != style)
            {
                Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, newStyle);
                Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0,
                    SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE | SWP.FRAMECHANGED);
            }
        }
    }

    private CaptionButtons _captionButtons = CaptionButtons.All;
    public CaptionButtons CaptionButtons
    {
        get => _captionButtons;
        set
        {
            if (_captionButtons == value) return;
            _captionButtons = value;
            if (Hwnd == 0) return;
            int style = Core.Win32.GetWindowLongW(Hwnd, GWL.STYLE);
            int newStyle = style;
            if (value.HasFlag(CaptionButtons.Close)) newStyle |= (int)WS.SYSMENU; else newStyle &= ~(int)WS.SYSMENU;
            if (value.HasFlag(CaptionButtons.Minimize)) newStyle |= (int)WS.MINIMIZEBOX; else newStyle &= ~(int)WS.MINIMIZEBOX;
            if (value.HasFlag(CaptionButtons.Maximize)) newStyle |= (int)WS.MAXIMIZEBOX; else newStyle &= ~(int)WS.MAXIMIZEBOX;
            if (newStyle != style)
            {
                Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, newStyle);
                Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0,
                    SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE | SWP.FRAMECHANGED);
            }
        }
    }

    // Phase D: Z-order
    public void BringToFront()
    {
        if (Hwnd == 0) return;
        // HWND_TOP = 0
        Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
    }

    public void SendToBack()
    {
        if (Hwnd == 0) return;
        // HWND_BOTTOM = 1
        Core.Win32.SetWindowPos(Hwnd, 1, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
    }

    /// <summary>Aplica <paramref name="minmax"/>: MinSize/MaxSize desde Layout + MaximizedBounds del monitor.</summary>
    internal void ApplyMinMaxInfo(ref MINMAXINFO info)
    {
        if (_minSize is { } ms)
        {
            info.ptMinTrackSize.X = ms.W;
            info.ptMinTrackSize.Y = ms.H;
        }
        if (_maxSize is { } mx)
        {
            info.ptMaxTrackSize.X = mx.W;
            info.ptMaxTrackSize.Y = mx.H;
        }
        // MaximizedBounds: área útil del monitor (taskbar excluida)
        nint mon = Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
        if (mon != 0)
        {
            MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
            if (Core.Win32.GetMonitorInfoW(mon, ref mi))
            {
                info.ptMaxSize.X = mi.rcWork.Right - mi.rcWork.Left;
                info.ptMaxSize.Y = mi.rcWork.Bottom - mi.rcWork.Top;
                info.ptMaxPosition.X = mi.rcWork.Left - mi.rcMonitor.Left;
                info.ptMaxPosition.Y = mi.rcWork.Top - mi.rcMonitor.Top;
            }
        }
    }

    /// <summary>Sincroniza <see cref="State"/> desde SYSCOMMAND (pulsaciones del caption).</summary>
    internal void SyncStateFromSysCommand(uint sysCmd)
    {
        _state = sysCmd switch
        {
            SC.MINIMIZE => WindowState.Minimized,
            SC.MAXIMIZE => WindowState.Maximized,
            SC.RESTORE => WindowState.Normal,
            _ => _state,
        };
    }

    internal void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    internal void RaiseActivated() => Activated?.Invoke(this, EventArgs.Empty);
    internal void RaiseDeactivated() => Deactivated?.Invoke(this, EventArgs.Empty);
    public void ScrollTo(int x, int y)
    {
        if (Hwnd == 0) return;
        int newX = Math.Clamp(x, 0, Math.Max(0, _maxScrollX));
        int newY = Math.Clamp(y, 0, Math.Max(0, _maxScrollY));
        if (newX == _scrollX && newY == _scrollY) return;
        _scrollX = newX;
        _scrollY = newY;

        // Actualizar SCROLLINFO
        if (_scrollConfig is not null)
        {
            if (_maxScrollY > 0 || _scrollConfig.VerticalScrollBarVisibility is ScrollBarVisibility.Always)
            {
                var siV = new SCROLLINFO { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(), fMask = SIF.POS, nPos = _scrollY };
                Core.Win32.SetScrollInfo(Hwnd, 1, ref siV, true);
            }
            if (_maxScrollX > 0 || _scrollConfig.HorizontalScrollBarVisibility is ScrollBarVisibility.Always)
            {
                var siH = new SCROLLINFO { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(), fMask = SIF.POS, nPos = _scrollX };
                Core.Win32.SetScrollInfo(Hwnd, 0, ref siH, true);
            }
        }

        // Notificar a MasterRouter para offsets de background
        // Se busca router vía HandleRegistry (si está registrado)
        var router = HandleRegistry.GetRouter(Hwnd);
        router?.SetScrollOffset(Hwnd, _scrollX, _scrollY);

        ApplyScrollOffset();
    }

    private void ApplyScrollOffset()
    {
        if (_materializedChildren is null || _materializedChildren.Count == 0) return;
        nint hdwp = Core.Win32.BeginDeferWindowPos(_materializedChildren.Count);
        bool batching = hdwp != 0;
        if (batching)
        {
            foreach (var child in _materializedChildren)
            {
                if (child is ControlBase cb && cb.Hwnd != 0)
                {
                    int x = (int)cb._arrangedX - _scrollX;
                    int y = (int)cb._arrangedY - _scrollY;
                    nint next = Core.Win32.DeferWindowPos(hdwp, cb.Hwnd, 0, x, y, (int)cb._arrangedW, (int)cb._arrangedH, SWP.NOZORDER | SWP.NOACTIVATE);
                    if (next != 0) hdwp = next;
                    else batching = false;
                }
            }
            if (batching)
            {
                Core.Win32.EndDeferWindowPos(hdwp);
                return;
            }
            if (hdwp != 0) Core.Win32.EndDeferWindowPos(hdwp);
        }
        foreach (var child in _materializedChildren)
        {
            if (child is ControlBase cb && cb.Hwnd != 0)
            {
                Core.Win32.SetWindowPos(cb.Hwnd, 0, (int)cb._arrangedX - _scrollX, (int)cb._arrangedY - _scrollY, (int)cb._arrangedW, (int)cb._arrangedH, SWP.NOZORDER | SWP.NOACTIVATE);
            }
        }
    }

    internal void HandleVScroll(int request, int thumbPos)
    {
        if (_scrollConfig is null) return;

        int newPos = _scrollY;
        switch (request)
        {
            case 0: // SB_LINEUP
                newPos = _scrollY - 20; break;
            case 1: // SB_LINEDOWN
                newPos = _scrollY + 20; break;
            case 2: // SB_PAGEUP
                newPos = _scrollY - _clientH; break;
            case 3: // SB_PAGEDOWN
                newPos = _scrollY + _clientH; break;
            case 4: // SB_THUMBPOSITION
            case 5: // SB_THUMBTRACK
                newPos = thumbPos; break;
            case 6: // SB_TOP
                newPos = 0; break;
            case 7: // SB_BOTTOM
                newPos = _maxScrollY; break;
            case 8: // SB_ENDSCROLL
                return;
            default: return;
        }
        ScrollTo(_scrollX, newPos);
    }

    internal void HandleHScroll(int request, int thumbPos)
    {
        if (_scrollConfig is null) return;
        int newPos = _scrollX;
        switch (request)
        {
            case 0: // SB_LINELEFT
                newPos = _scrollX - 20; break;
            case 1: // SB_LINERIGHT
                newPos = _scrollX + 20; break;
            case 2: // SB_PAGELEFT
                newPos = _scrollX - _clientW; break;
            case 3: // SB_PAGERIGHT
                newPos = _scrollX + _clientW; break;
            case 4:
            case 5:
                newPos = thumbPos; break;
            case 6:
                newPos = 0; break;
            case 7:
                newPos = _maxScrollX; break;
            case 8:
                return;
            default: return;
        }
        ScrollTo(newPos, _scrollY);
    }

    internal void HandleMouseWheel(int delta)
    {
        if (_scrollConfig is null) return;
        // Rueda vertical por defecto; si orientación es Horizontal, scrollear horizontal
        bool isHorizontal = _scrollConfig.Orientation is ScrollOrientation.Horizontal;
        if (isHorizontal)
        {
            int newX = _scrollX - delta * 30 / 120;
            ScrollTo(newX, _scrollY);
        }
        else
        {
            int newY = _scrollY - delta * 30 / 120;
            ScrollTo(_scrollX, newY);
        }
    }

    private void ConfigureScrollbars()
    {
        if (_scrollConfig is null || Hwnd == 0) return;

        // Calcular tamaños totales del contenido
        int totalH = 0;
        int totalW = 0;
        if (_materializedChildren is not null && _materializedChildren.Count > 0)
        {
            // Altura total para VerticalStack
            foreach (var c in _materializedChildren)
            {
                totalH += (int)(c.MeasuredHeight + c.Margin.Top + c.Margin.Bottom);
                totalW = Math.Max(totalW, (int)(c.MeasuredWidth + c.Margin.Left + c.Margin.Right));
            }
            if (_materializedChildren.Count > 1) totalH += (int)(_contentModel!.Spacing * (_materializedChildren.Count - 1));
            totalH += (int)(_contentModel!.Padding.Top + _contentModel.Padding.Bottom);
            totalW += (int)(_contentModel.Padding.Left + _contentModel.Padding.Right);
        }

        bool wantsV = _scrollConfig.Orientation == ScrollOrientation.Vertical || _scrollConfig.Orientation == ScrollOrientation.Both;
        bool wantsH = _scrollConfig.Orientation == ScrollOrientation.Horizontal || _scrollConfig.Orientation == ScrollOrientation.Both;

        bool needV = false, needH = false;
        if (wantsV)
        {
            needV = totalH > _clientH;
            if (_scrollConfig.VerticalScrollBarVisibility is ScrollBarVisibility.Never) needV = false;
            else if (_scrollConfig.VerticalScrollBarVisibility is ScrollBarVisibility.Always) needV = true;
        }
        if (wantsH)
        {
            needH = totalW > _clientW;
            if (_scrollConfig.HorizontalScrollBarVisibility is ScrollBarVisibility.Never) needH = false;
            else if (_scrollConfig.HorizontalScrollBarVisibility is ScrollBarVisibility.Always) needH = true;
        }

        // Activar estilos WS_HSCROLL/VSCROLL
        int style = Core.Win32.GetWindowLongW(Hwnd, GWL.STYLE);
        int newStyle = style;
        if (needV) newStyle |= (int)WS.VSCROLL; else newStyle &= ~(int)WS.VSCROLL;
        if (needH) newStyle |= (int)WS.HSCROLL; else newStyle &= ~(int)WS.HSCROLL;
        if (newStyle != style)
        {
            Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, newStyle);
            Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE | SWP.FRAMECHANGED);
        }

        _maxScrollY = Math.Max(0, totalH - _clientH);
        _maxScrollX = Math.Max(0, totalW - _clientW);

        // Clampear offset actual
        _scrollX = Math.Clamp(_scrollX, 0, _maxScrollX);
        _scrollY = Math.Clamp(_scrollY, 0, _maxScrollY);

        // Configurar SCROLLINFO
        if (needV || _scrollConfig.VerticalScrollBarVisibility is ScrollBarVisibility.Always)
        {
            var si = new SCROLLINFO
            {
                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(),
                fMask = SIF.RANGE | SIF.PAGE | SIF.POS,
                nMin = 0,
                nMax = Math.Max(0, totalH - 1),
                nPage = (uint)Math.Max(0, _clientH),
                nPos = _scrollY
            };
            Core.Win32.SetScrollInfo(Hwnd, 1, ref si, true);
        }
        if (needH || _scrollConfig.HorizontalScrollBarVisibility is ScrollBarVisibility.Always)
        {
            var si = new SCROLLINFO
            {
                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(),
                fMask = SIF.RANGE | SIF.PAGE | SIF.POS,
                nMin = 0,
                nMax = Math.Max(0, totalW - 1),
                nPage = (uint)Math.Max(0, _clientW),
                nPos = _scrollX
            };
            Core.Win32.SetScrollInfo(Hwnd, 0, ref si, true);
        }

        // Sincronizar con MasterRouter para dibujado
        var router = HandleRegistry.GetRouter(Hwnd);
        router?.SetScrollOffset(Hwnd, _scrollX, _scrollY);

        // Aplicar offset a hijos si hay scroll
        if (_scrollX != 0 || _scrollY != 0) ApplyScrollOffset();
    }

    internal void RaiseResizing(float w, float h) => Resizing?.Invoke(this, new WindowResizingEventArgs(w, h));
    internal void RaiseResized(float w, float h)
    {
        // Actualizar backing fields para Width/Height (evitar recursión SetWindowPos)
        _width = w;
        _height = h;
        _clientW = (int)w;
        _clientH = (int)h;

        if (_materializedChildren is not null && _contentModel is not null && _materializedChildren.Count > 0)
        {
            var engine = new Core.LayoutEngine.LayoutEngine(new VerticalStackLayoutStrategy());
            engine.Execute(_materializedChildren, w, h, _contentModel.Spacing, _contentModel.Padding);
        }

        // Reconfigurar scrollbars tras resize (puede cambiar necesidad de scroll)
        if (_scrollConfig is not null) ConfigureScrollbars();

        Resized?.Invoke(this, new WindowResizedEventArgs((int)w, (int)h));
    }
    internal void RaiseMoved(int x, int y) => Moved?.Invoke(this, new WindowMovedEventArgs(x, y));

    internal void MaterializeContent(WindowModel window, HandleRegistry registry, MasterRouter router)
    {
        if (window.Content is not ContentModel content) return;
        if (content.Children.Count == 0) return;

        var layoutables = new List<ILayoutable>();
        MaterializeChildren(content, Hwnd, registry, router, layoutables);

        if (layoutables.Count == 0) return;

        // Calcular área disponible (client rect)
        Win32.GetClientRect(Hwnd, out RECT rect);
        float availW = rect.Right - rect.Left;
        float availH = rect.Bottom - rect.Top;
        if (availW <= 0) availW = window.Width;
        if (availH <= 0) availH = window.Height;

        var engine = new Core.LayoutEngine.LayoutEngine(new VerticalStackLayoutStrategy());
        engine.Execute(layoutables, availW, availH, content.Spacing, content.Padding);

        _materializedChildren = layoutables;
        _contentModel = content;
        _clientW = (int)availW;
        _clientH = (int)availH;

        // Fase 4: Scroll
        if (window.ScrollConfig is not null)
        {
            _scrollConfig = window.ScrollConfig;
            _scrollX = (int)window.ScrollConfig.ScrollX;
            _scrollY = (int)window.ScrollConfig.ScrollY;
            ConfigureScrollbars();
            // Si hay offset inicial, aplicar
            if (_scrollX != 0 || _scrollY != 0) ScrollTo(_scrollX, _scrollY);
        }
    }

    private static void MaterializeChildren(ContentModel model, nint parentHwnd, HandleRegistry registry, MasterRouter router, List<ILayoutable> outLayoutables)
    {
        foreach (var vmBase in model.Children)
        {
            if (vmBase is not ViewModel vm) continue;
            var control = vm.Control;
            if (control is null)
            {
                if (vm.ControlType is null) continue;
                control = (IControl)ControlActivatorRegistry.Shared.CreateFor(vm.ControlType);
                // Asignar Router/Registry antes de Configure para que OnMessage tenga contexto (mejora sobre referencia del plan)
                if (control is ControlBase cbPre)
                {
                    cbPre.Router = router;
                    cbPre.Registry = registry;
                }
                vm.Configure?.Invoke(control);
                vm.Control = control;
            }

            // Las superficies sin HWND (IMenu, IMenuItem) no se materializan aquí;
            // las materializa MenuSurface (Fase 1/2).
            if (control is not IControl nativeControl) continue;

            // Resolver factory por tipo concreto/interfaz
            if (!ControlActivatorRegistry.Shared.TryGetFactoryForControl(nativeControl, out var factory) || factory is null)
            {
                if (vm.ControlType is not null && ControlActivatorRegistry.Shared.TryGetFactory(vm.ControlType) is { } fallback && fallback is not null)
                    factory = fallback;
                else
                    throw new InvalidOperationException($"No handle factory registered for control '{control.GetType().Name}' (name='{control.Name}'). Registre un INativeHandleFactory para ese tipo.");
            }

            nint hwnd = factory.CreateHandle(parentHwnd, nativeControl, registry);
            if (hwnd == 0)
                throw new InvalidOperationException($"CreateHandle failed for control '{control.Name}' ({control.GetType().Name})");

            if (control is ControlBase cb)
            {
                cb.Router = router;
                cb.Registry = registry;

                // Registrar brush si tiene Background
                nint brush = cb.GetOrCreateBackgroundBrush();
                if (brush != 0)
                    router.RegisterControlBrush(hwnd, brush);
            }

            outLayoutables.Add((ILayoutable)control);

            // Recursión para SubContent (contenedores anidados)
            if (vm.SubContent is ContentModel sub)
            {
                // Los hijos de SubContent son hijos visuales de 'control'
                // Se materializan bajo hwnd del control padre, pero NO se añaden al layout del window
                // El layout interno será gestionado por el control contenedor (ej. UserControl.PostRender) en Fase 2 deja batch simple para ellos
                var subLayoutables = new List<ILayoutable>();
                MaterializeChildren(sub, hwnd, registry, router, subLayoutables);

                // Si el control es UserControl, inyectar hijos y hacer layout inmediato del contenedor
                if (control is UserControl uc)
                {
                    foreach (var subChild in subLayoutables)
                        uc.Children.Add(subChild);
                    uc.Spacing = sub.Spacing;
                    // Padding del UserControl ya viene de View<T>.Padding, pero también aplicar el Padding del sub-content si existe
                    // Layout interno: se hará en PostRender, pero podemos hacer un primer layout usando GetClientRect del hwnd recién creado
                    // (opcional, PostRender lo repetirá al primer Render)
                }
                else if (subLayoutables.Count > 0)
                {
                    // Para controles no contenedor, tratar sub-hijos como hermanos adicionales bajo la ventana
                    // (fallback) — no debería ocurrir en uso normal
                    outLayoutables.AddRange(subLayoutables);
                }
            }
        }
    }
}
