using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core;
using EasyWindowsApplication.Core.LayoutEngine;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Core.Windowing;

internal sealed class WindowImpl : IWindow
{
    public nint Hwnd { get; }
    public string Name { get; }
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value ?? "";
            if (Hwnd != 0) EasyWindowsApplication.Core.Win32.SetWindowText(Hwnd, _title);
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
                EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, (int)_width, (int)_height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
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
                EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, (int)_width, (int)_height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
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
    public event EventHandler<CancelEventArgs>? Closing;
    public event EventHandler? Closed;
    public event EventHandler? Activated;
    public event EventHandler? Deactivated;
    public event EventHandler<WindowResizingEventArgs>? Resizing;
    public event EventHandler<WindowResizedEventArgs>? Resized;
    public event EventHandler<WindowMovedEventArgs>? Moved;

    internal WindowImpl(nint hwnd, string name, string title, float width, float height, WindowPositionOnScreen position)
    {
        Hwnd = hwnd;
        Name = name;
        _title = title ?? "";
        _width = width;
        _height = height;
        _positionMode = position;
    }

    public void Show() => EasyWindowsApplication.Core.Windowing.Win32.ShowWindow(Hwnd, SW.SHOW);
    public void Hide() => EasyWindowsApplication.Core.Windowing.Win32.ShowWindow(Hwnd, SW.HIDE);
    public void Close() => EasyWindowsApplication.Core.Windowing.Win32.DestroyWindow(Hwnd);

    public void Center()
    {
        if (Hwnd == 0) return;
        nint monitor = EasyWindowsApplication.Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
        if (monitor == 0) monitor = EasyWindowsApplication.Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTOPRIMARY);
        if (monitor == 0) return;

        MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
        if (!EasyWindowsApplication.Core.Win32.GetMonitorInfoW(monitor, ref mi)) return;

        EasyWindowsApplication.Core.Win32.GetWindowRect(Hwnd, out RECT wr);
        int winW = wr.Right - wr.Left;
        int winH = wr.Bottom - wr.Top;

        int workW = mi.rcWork.Right - mi.rcWork.Left;
        int workH = mi.rcWork.Bottom - mi.rcWork.Top;

        int x = mi.rcWork.Left + (workW - winW) / 2;
        int y = mi.rcWork.Top + (workH - winH) / 2;

        EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
    }

    public void Maximize() => EasyWindowsApplication.Core.Win32.ShowWindow(Hwnd, SW.MAXIMIZE);
    public void Minimize() => EasyWindowsApplication.Core.Win32.ShowWindow(Hwnd, SW.MINIMIZE);
    public void Restore() => EasyWindowsApplication.Core.Win32.ShowWindow(Hwnd, SW.RESTORE);
    public void Focus() => EasyWindowsApplication.Core.Win32.SetForegroundWindow(Hwnd);

    internal void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    internal bool RaiseClosing()
    {
        var args = new CancelEventArgs();
        Closing?.Invoke(this, args);
        return args.Cancel;
    }
    internal void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
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
        if (_scrollConfig != null)
        {
            if (_maxScrollY > 0 || _scrollConfig.VerticalScrollBarVisibility == ScrollBarVisibility.Always)
            {
                var siV = new SCROLLINFO { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(), fMask = SIF.POS, nPos = _scrollY };
                EasyWindowsApplication.Core.Win32.SetScrollInfo(Hwnd, 1, ref siV, true);
            }
            if (_maxScrollX > 0 || _scrollConfig.HorizontalScrollBarVisibility == ScrollBarVisibility.Always)
            {
                var siH = new SCROLLINFO { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<SCROLLINFO>(), fMask = SIF.POS, nPos = _scrollX };
                EasyWindowsApplication.Core.Win32.SetScrollInfo(Hwnd, 0, ref siH, true);
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
        if (_materializedChildren == null || _materializedChildren.Count == 0) return;
        nint hdwp = EasyWindowsApplication.Core.Win32.BeginDeferWindowPos(_materializedChildren.Count);
        bool batching = hdwp != 0;
        if (batching)
        {
            foreach (var child in _materializedChildren)
            {
                if (child is ControlBase cb && cb.Hwnd != 0)
                {
                    int x = (int)cb._arrangedX - _scrollX;
                    int y = (int)cb._arrangedY - _scrollY;
                    nint next = EasyWindowsApplication.Core.Win32.DeferWindowPos(hdwp, cb.Hwnd, 0, x, y, (int)cb._arrangedW, (int)cb._arrangedH, SWP.NOZORDER | SWP.NOACTIVATE);
                    if (next != 0) hdwp = next;
                    else batching = false;
                }
            }
            if (batching)
            {
                EasyWindowsApplication.Core.Win32.EndDeferWindowPos(hdwp);
                return;
            }
            if (hdwp != 0) EasyWindowsApplication.Core.Win32.EndDeferWindowPos(hdwp);
        }
        foreach (var child in _materializedChildren)
        {
            if (child is ControlBase cb && cb.Hwnd != 0)
            {
                EasyWindowsApplication.Core.Win32.SetWindowPos(cb.Hwnd, 0, (int)cb._arrangedX - _scrollX, (int)cb._arrangedY - _scrollY, (int)cb._arrangedW, (int)cb._arrangedH, SWP.NOZORDER | SWP.NOACTIVATE);
            }
        }
    }

    internal void HandleVScroll(int request, int thumbPos)
    {
        if (_scrollConfig == null) return;
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
        if (_scrollConfig == null) return;
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
        if (_scrollConfig == null) return;
        // Rueda vertical por defecto; si orientación es Horizontal, scrollear horizontal
        bool isHorizontal = _scrollConfig.Orientation == ScrollOrientation.Horizontal;
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
        if (_scrollConfig == null || Hwnd == 0) return;

        // Calcular tamaños totales del contenido
        int totalH = 0;
        int totalW = 0;
        if (_materializedChildren != null && _materializedChildren.Count > 0)
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
            if (_scrollConfig.VerticalScrollBarVisibility == ScrollBarVisibility.Never) needV = false;
            else if (_scrollConfig.VerticalScrollBarVisibility == ScrollBarVisibility.Always) needV = true;
        }
        if (wantsH)
        {
            needH = totalW > _clientW;
            if (_scrollConfig.HorizontalScrollBarVisibility == ScrollBarVisibility.Never) needH = false;
            else if (_scrollConfig.HorizontalScrollBarVisibility == ScrollBarVisibility.Always) needH = true;
        }

        // Activar estilos WS_HSCROLL/VSCROLL
        int style = EasyWindowsApplication.Core.Win32.GetWindowLongW(Hwnd, GWL.STYLE);
        int newStyle = style;
        if (needV) newStyle |= (int)WS.VSCROLL; else newStyle &= ~(int)WS.VSCROLL;
        if (needH) newStyle |= (int)WS.HSCROLL; else newStyle &= ~(int)WS.HSCROLL;
        if (newStyle != style)
        {
            EasyWindowsApplication.Core.Win32.SetWindowLongW(Hwnd, GWL.STYLE, newStyle);
            EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOSIZE | SWP.FRAMECHANGED);
        }

        _maxScrollY = Math.Max(0, totalH - _clientH);
        _maxScrollX = Math.Max(0, totalW - _clientW);

        // Clampear offset actual
        _scrollX = Math.Clamp(_scrollX, 0, _maxScrollX);
        _scrollY = Math.Clamp(_scrollY, 0, _maxScrollY);

        // Configurar SCROLLINFO
        if (needV || _scrollConfig.VerticalScrollBarVisibility == ScrollBarVisibility.Always)
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
            EasyWindowsApplication.Core.Win32.SetScrollInfo(Hwnd, 1, ref si, true);
        }
        if (needH || _scrollConfig.HorizontalScrollBarVisibility == ScrollBarVisibility.Always)
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
            EasyWindowsApplication.Core.Win32.SetScrollInfo(Hwnd, 0, ref si, true);
        }

        // Sincronizar con MasterRouter para dibujado
        var router = HandleRegistry.GetRouter(Hwnd);
        router?.SetScrollOffset(Hwnd, _scrollX, _scrollY);

        // Aplicar offset a hijos si hay scroll
        if (_scrollX != 0 || _scrollY != 0) ApplyScrollOffset();
    }

    internal void RaiseResizing(int w, int h) => Resizing?.Invoke(this, new WindowResizingEventArgs(w, h));
    internal void RaiseResized(int w, int h)
    {
        // Actualizar backing fields para Width/Height (evitar recursión SetWindowPos)
        _width = w;
        _height = h;
        _clientW = w;
        _clientH = h;

        if (_materializedChildren != null && _contentModel != null && _materializedChildren.Count > 0)
        {
            var engine = new EasyWindowsApplication.Core.LayoutEngine.LayoutEngine(new VerticalStackLayoutStrategy());
            engine.Execute(_materializedChildren, w, h, _contentModel.Spacing, _contentModel.Padding);
        }

        // Reconfigurar scrollbars tras resize (puede cambiar necesidad de scroll)
        if (_scrollConfig != null) ConfigureScrollbars();

        Resized?.Invoke(this, new WindowResizedEventArgs(w, h));
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

        var engine = new EasyWindowsApplication.Core.LayoutEngine.LayoutEngine(new VerticalStackLayoutStrategy());
        engine.Execute(layoutables, availW, availH, content.Spacing, content.Padding);

        _materializedChildren = layoutables;
        _contentModel = content;
        _clientW = (int)availW;
        _clientH = (int)availH;

        // Fase 4: Scroll
        if (window.ScrollConfig != null)
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

            // Resolver factory por tipo concreto/interfaz
            if (!ControlActivatorRegistry.Shared.TryGetFactoryForControl(control, out var factory) || factory is null)
            {
                if (vm.ControlType is not null && ControlActivatorRegistry.Shared.TryGetFactory(vm.ControlType) is { } fallback && fallback is not null)
                    factory = fallback;
                else
                    throw new InvalidOperationException($"No handle factory registered for control '{control.GetType().Name}' (name='{control.Name}'). Registre un INativeHandleFactory para ese tipo.");
            }

            nint hwnd = factory.CreateHandle(parentHwnd, control, registry);
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
