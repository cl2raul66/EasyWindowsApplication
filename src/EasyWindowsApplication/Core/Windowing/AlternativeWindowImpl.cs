using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core;
using EasyWindowsApplication.Core.LayoutEngine;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Core.Windowing;

internal sealed class AlternativeWindowImpl : IAlternativeWindow
{
    public nint Hwnd { get; }
    public string Name { get; }
    public nint OwnerHwnd { get; }
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
    private int _width;
    public int Width
    {
        get => _width;
        set
        {
            if (_width == value) return;
            _width = value;
            if (Hwnd != 0)
                EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, _width, _height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
        }
    }
    private int _height;
    public int Height
    {
        get => _height;
        set
        {
            if (_height == value) return;
            _height = value;
            if (Hwnd != 0)
                EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, 0, 0, _width, _height, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOMOVE);
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
            {
                // Reuse Center logic similar to WindowImpl
                nint monitor = EasyWindowsApplication.Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
                if (monitor == 0) monitor = EasyWindowsApplication.Core.Win32.MonitorFromWindow(Hwnd, MONITOR.DEFAULTTOPRIMARY);
                if (monitor != 0)
                {
                    MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
                    if (EasyWindowsApplication.Core.Win32.GetMonitorInfoW(monitor, ref mi))
                    {
                        EasyWindowsApplication.Core.Win32.GetWindowRect(Hwnd, out RECT wr);
                        int winW = wr.Right - wr.Left;
                        int winH = wr.Bottom - wr.Top;
                        int workW = mi.rcWork.Right - mi.rcWork.Left;
                        int workH = mi.rcWork.Bottom - mi.rcWork.Top;
                        int x = mi.rcWork.Left + (workW - winW) / 2;
                        int y = mi.rcWork.Top + (workH - winH) / 2;
                        EasyWindowsApplication.Core.Win32.SetWindowPos(Hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
                    }
                }
            }
        }
    }

    private List<ILayoutable>? _materializedChildren;
    private ContentModel? _contentModel;
    private bool _hasLoaded;

    public event EventHandler? Loaded;
    public event EventHandler<CancelEventArgs>? Closing;
    public event EventHandler? Closed;
    public event EventHandler? Activated;
    public event EventHandler? Deactivated;

    internal AlternativeWindowImpl(nint hwnd, nint ownerHwnd, string name, string title, int width, int height, WindowPositionOnScreen position)
    {
        Hwnd = hwnd;
        OwnerHwnd = ownerHwnd;
        Name = name;
        _title = title ?? "";
        _width = width;
        _height = height;
        _positionMode = position;
    }

    public void Show()
    {
        EasyWindowsApplication.Core.Windowing.Win32.ShowWindow(Hwnd, SW.SHOW);
        if (!_hasLoaded)
        {
            _hasLoaded = true;
            Loaded?.Invoke(this, EventArgs.Empty);
        }
    }
    public void Hide() => EasyWindowsApplication.Core.Windowing.Win32.ShowWindow(Hwnd, SW.HIDE);
    public void Close() => EasyWindowsApplication.Core.Windowing.Win32.DestroyWindow(Hwnd);

    internal void RaiseLoaded()
    {
        if (_hasLoaded) return;
        _hasLoaded = true;
        Loaded?.Invoke(this, EventArgs.Empty);
    }
    internal bool RaiseClosing()
    {
        var args = new CancelEventArgs();
        Closing?.Invoke(this, args);
        return args.Cancel;
    }
    internal void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
    internal void RaiseActivated() => Activated?.Invoke(this, EventArgs.Empty);
    internal void RaiseDeactivated() => Deactivated?.Invoke(this, EventArgs.Empty);

    internal void MaterializeContent(WindowModel window, HandleRegistry registry, MasterRouter router)
    {
        if (window.Content is not ContentModel content) return;
        if (content.Children.Count == 0) return;

        var layoutables = new List<ILayoutable>();
        MaterializeChildren(content, Hwnd, registry, router, layoutables);

        if (layoutables.Count == 0) return;

        Win32.GetClientRect(Hwnd, out RECT rect);
        float availW = rect.Right - rect.Left;
        float availH = rect.Bottom - rect.Top;
        if (availW <= 0) availW = window.Width;
        if (availH <= 0) availH = window.Height;

        var engine = new EasyWindowsApplication.Core.LayoutEngine.LayoutEngine(new VerticalStackLayoutStrategy());
        engine.Execute(layoutables, availW, availH, content.Spacing, content.Padding);

        _materializedChildren = layoutables;
        _contentModel = content;
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
                if (control is ControlBase cbPre)
                {
                    cbPre.Router = router;
                    cbPre.Registry = registry;
                }
                vm.Configure?.Invoke(control);
                vm.Control = control;
            }

            if (!ControlActivatorRegistry.Shared.TryGetFactoryForControl(control, out var factory) || factory is null)
            {
                if (vm.ControlType is not null && ControlActivatorRegistry.Shared.TryGetFactory(vm.ControlType) is { } fallback)
                    factory = fallback;
                else
                    throw new InvalidOperationException($"No handle factory registered for control '{control.GetType().Name}' (name='{control.Name}').");
            }

            nint hwnd = factory.CreateHandle(parentHwnd, control, registry);
            if (hwnd == 0)
                throw new InvalidOperationException($"CreateHandle failed for control '{control.Name}' ({control.GetType().Name})");

            if (control is ControlBase cb)
            {
                cb.Router = router;
                cb.Registry = registry;
                nint brush = cb.GetOrCreateBackgroundBrush();
                if (brush != 0) router.RegisterControlBrush(hwnd, brush);
            }

            outLayoutables.Add((ILayoutable)control);

            if (vm.SubContent is ContentModel sub)
            {
                var subLayoutables = new List<ILayoutable>();
                MaterializeChildren(sub, hwnd, registry, router, subLayoutables);
                if (control is UserControl uc)
                {
                    foreach (var subChild in subLayoutables) uc.Children.Add(subChild);
                    uc.Spacing = sub.Spacing;
                }
                else if (subLayoutables.Count > 0)
                {
                    outLayoutables.AddRange(subLayoutables);
                }
            }
        }
    }
}
