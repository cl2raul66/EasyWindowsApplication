using System.Diagnostics;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Core.LayoutEngine;
using EasyWindowsApplication.Core.Windowing;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.Core;

internal sealed class Application :
    IApplicationLayoutPhase,
    IApplicationPostLayoutPhase,
    IApplicationPostBehaviorPhase
{
    internal ResourcesDictionaryImpl ResourcesDictionary { get; } = new();
    internal BehaviorBuilderImpl BehaviorBuilder { get; } = new();

    private readonly List<WindowModel> _windows = new();
    private MasterRouter _router = null!;
    private readonly HandleRegistry _registry = new();
    private Action<IBehaviorBuilder>? _pendingBehavior;
    private nint _mainHwnd;
    private SystemTray.SystemTrayBroker? _trayBroker;
    private readonly List<Menus.MenuSurface> _pendingMenuSurfaces = new();

    public IApplicationLayoutPhase Resources(Action<IResourcesDictionary> configure)
    {
        configure(ResourcesDictionary);
        return this;
    }

    public IApplicationPostLayoutPhase Layout() => this;

    public IApplicationPostLayoutPhase Layout(Action<ILayoutBuilder> configure)
    {
        var builder = new LayoutBuilderImpl(this);
        configure(builder);
        return this;
    }

    public IApplicationPostBehaviorPhase Behavior() => this;

    public IApplicationPostBehaviorPhase Behavior(Action<IBehaviorBuilder> configure)
    {
        _pendingBehavior += configure;
        return this;
    }

    internal void AddWindow(WindowModel window)
    {
        if (_windows.Count == 0)
            Win32ControlsModule.Backend.ControlProcedures.SetInstance(Win32.GetModuleHandleW(0));
        _windows.Add(window);
    }

    public void Initialize()
    {
        // 1. Registrar defaults PRIMERO (antes de cualquier control o HFONT)
        try { UiDefaultsProvider.Set(new Win32ControlsModule.Backend.Win32UiDefaults()); } catch { }

        var icc = new INITCOMMONCONTROLSEX
        {
            dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<INITCOMMONCONTROLSEX>(),
            dwICC = ICC.STANDARD_CLASSES
        };
        Win32.InitCommonControlsEx(ref icc);

        ControlActivatorRegistry.EnsureInitialized();
        _router = new MasterRouter(_registry);

        Debug.Assert(
            UiDefaultsProvider.IsInitialized,
            "UiDefaultsProvider debe inicializarse (UseWinApi / UseWpf / …) " +
            "antes de registrar la clase de ventana.");

        foreach (var window in _windows)
        {
            if (window.IsAlternative)
                RegisterAlternative(window);
            else
                RegisterMain(window);
        }

        if (_pendingBehavior is not null)
        {
            BehaviorBuilder.Registry = _registry;
            BehaviorBuilder.MainHwnd = _mainHwnd;
            ControlAccess.SetController(BehaviorBuilder);
            _pendingBehavior(BehaviorBuilder);
            BehaviorBuilder.RaiseLaunched();
        }

        Procedures.RunMessageLoop();
    }

    private void RegisterMain(WindowModel window)
    {
        var hwnd = Core.Windowing.Procedures.CreateMainWindow(_router, window.Title, window.Width, window.Height);
        _mainHwnd = hwnd;

        var win = new Core.Windowing.WindowImpl(
            hwnd, window.Name, window.Title, window.Width, window.Height, window.Position);

        if (window.Background.HasValue)
        {
            var brush = Win32.CreateSolidBrush(window.Background.Value.ToCOLORREF());
            _router.RegisterWindowBackgroundBrush(hwnd, brush);
        }

        _registry.RegisterWindow(win);
        // Fase 2: materializar contenido (creación de HWNDs hijos + layout batching sin flicker)
        win.MaterializeContent(window, _registry, _router);
        // Si Position == Center, centrar antes de mostrar (multi-monitor correcto)
        if (window.Position == WindowPositionOnScreen.Center)
            win.Center();
        win.Show();
        win.RaiseLoaded();
        if (window.SystemTrayConfigure is { } trayConfigure)
            SetupSystemTrayIcon(trayConfigure, hwnd);
    }

    private void SetupSystemTrayIcon(Action<ISystemTray> configure, nint mainHwnd)
    {
        SystemTray.SystemTrayImpl? tray = null;
        var broker = SystemTray.SystemTrayBroker.Create(_router, (trigger, count) => tray?.DispatchTrigger(trigger, count));
        tray = new SystemTray.SystemTrayImpl(broker);
        configure(tray);
        nint hIcon = Core.Windowing.Procedures.LoadAppIcon();
        broker.AddIcon(hIcon);
        _registry.RegisterSurface(tray.Name, tray);
        _trayBroker = broker;
        WireMenuSurfaces();
        _router.RegisterHandler(mainHwnd, WM.DESTROY, (w, l) => { broker.Shutdown(); return 0; });
    }

    private void WireMenuSurfaces()
    {
        if (_trayBroker is null) return;
        foreach (var surface in _pendingMenuSurfaces)
        {
            surface.SetOwner(_trayBroker.Hwnd);
            surface.SetIconAnchor(_trayBroker.Hwnd, SystemTray.SystemTrayBroker.IconId);
        }
        _pendingMenuSurfaces.Clear();
    }

    private void RegisterAlternative(WindowModel window)
    {
        if (window.SurfaceType is not null && typeof(IMenu).IsAssignableFrom(window.SurfaceType))
        {
            var surface = new Menus.MenuSurface(window.Content as ContentModel);
            if (!string.IsNullOrEmpty(window.Name))
                surface.Name = window.Name;
            surface.Registry = _registry;
            surface.EnsureMaterialized();
            _registry.RegisterSurface(window.Name, surface);
            _pendingMenuSurfaces.Add(surface);
            WireMenuSurfaces();
            return;
        }
        var hwnd = Core.Windowing.Procedures.CreateAlternativeWindow(_router, 0, window.Title, window.Width, window.Height);
        var win = new Core.Windowing.AlternativeWindowImpl(hwnd, 0, window.Name, window.Title, window.Width, window.Height, window.Position);
        if (window.Background.HasValue)
        {
            var brush = Win32.CreateSolidBrush(window.Background.Value.ToCOLORREF());
            _router.RegisterWindowBackgroundBrush(hwnd, brush);
        }
        _registry.RegisterWindow(win);
        // Fase 2: materializar contenido para AlternativeWindow también
        win.MaterializeContent(window, _registry, _router);
        if (window.Position == WindowPositionOnScreen.Center)
        {
            // Center para AlternativeWindow (pre-calcula posición pero no muestra)
            nint monitor = Win32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
            if (monitor == 0) monitor = Win32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTOPRIMARY);
            if (monitor != 0)
            {
                MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
                if (Win32.GetMonitorInfoW(monitor, ref mi))
                {
                    Win32.GetWindowRect(hwnd, out RECT wr);
                    int winW = wr.Right - wr.Left;
                    int winH = wr.Bottom - wr.Top;
                    int x = mi.rcWork.Left + ((mi.rcWork.Right - mi.rcWork.Left - winW) / 2);
                    int y = mi.rcWork.Top + ((mi.rcWork.Bottom - mi.rcWork.Top - winH) / 2);
                    Win32.SetWindowPos(hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
                }
            }
        }
        // AlternativeWindow inicia oculta — se muestra explícitamente via GetWindow<IAlternativeWindow>(name).Show()
        // No se llama win.Show() ni win.RaiseLoaded() aquí (Loaded se disparará en el Show manual)
    }
}
