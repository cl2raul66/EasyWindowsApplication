using System.Diagnostics;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Core.LayoutEngine;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.Core;

internal sealed class Application :
    IApplicationLayoutPhase,
    IApplicationPostLayoutPhase,
    IApplicationPostBehaviorPhase
{
    internal ResourcesDictionaryImpl ResourcesDictionary { get; } = new();
    internal BehaviorBuilderImpl BehaviorBuilder { get; } = new();

    private readonly List<WindowModel> _windows = [];
    private MasterRouter _router = null!;
    private readonly HandleRegistry _registry = new();
    private Action<IBehaviorBuilder>? _pendingBehavior;
    private nint _mainHwnd;
    private SystemTray.SystemTrayBroker? _trayBroker;
    private readonly List<Menus.MenuSurface> _pendingMenuSurfaces = [];

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
        Threading.UiApartment.RunStaOrCurrent(RunPipeline);
    }

    private void RunPipeline()
    {
        // 1. Registrar defaults PRIMERO (antes de cualquier control o HFONT)
        try { UiDefaultsProvider.Set(new Win32ControlsModule.Backend.Win32UiDefaults()); } catch { }

        // Cultura explícita (Resources) o automática del OS (no tocar nada)
        var culture = ResourcesDictionary.SettingsContext.SelectedCulture;
        if (culture is not null)
        {
            System.Globalization.CultureInfo.CurrentCulture = culture;
            System.Globalization.CultureInfo.CurrentUICulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

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

        _router.MainHwnd = _mainHwnd;

        if (_pendingBehavior is not null)
        {
            BehaviorBuilder.Registry = _registry;
            BehaviorBuilder.MainHwnd = _mainHwnd;
            ControlAccess.SetController(BehaviorBuilder);
            _pendingBehavior(BehaviorBuilder);
            BehaviorBuilder.RaiseLaunched();
        }

        _router.TerminationGate = () => BehaviorBuilder.RaiseTerminating();

        Procedures.RunMessageLoop();

        // Teardown determinístico: destruir las secundarias vivas antes del
        // epílogo. DestroyWindow entrega WM_DESTROY síncronamente (sin pump).
        // Closing/Closed suprimidos: teardown de app, no cierre de ventana —
        // ningún callback de Behavior debe correr con la UI del usuario destruida.
        foreach (var live in _registry.LiveWindows())
        {
            if (live is Windowing.AlternativeWindowImpl { Hwnd: not 0 } alt)
            {
                alt.MarkTearingDown();
                Win32.DestroyWindow(alt.Hwnd);
            }
        }

        // Teardown total: entradas restantes (secundarias detachadas, superficies) salen por Remove.
        _registry.RemoveAll();

        BehaviorBuilder.RaiseTerminated();
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

        _registry.Attach(EntityKind.Window, win, win.Hwnd, win.Name);
        ApplyModelProps(win, window);
        ApplyWindowPosition(win, window);
        // Fase 2: materializar contenido (creación de HWNDs hijos + layout batching sin flicker)
        win.MaterializeContent(window, _registry, _router);
        // Si Position == Center, centrar antes de mostrar (multi-monitor correcto)
        if (window.Position == WindowPositionOnScreen.Center && window.WindowPosition is null)
            win.Center();
        // Estado inicial (Phase A — #47)
        if (window.InitialWindowState != Share.WindowState.Normal)
            win.State = window.InitialWindowState;
        win.Show();
        win.RaiseLoaded();
        if (window.SystemTrayConfigure is { } trayConfigure)
            SetupSystemTrayIcon(trayConfigure, hwnd);
    }

    /// <summary>
    /// Aplica propiedades del modelo al impl (Phase C — #47).
    /// Se usa tras la creación del HWND, antes de materializar contenido.
    /// </summary>
    private static void ApplyModelProps(Windowing.WindowImpl win, WindowModel model)
    {
        if (model.MinSize is { } ms) win.MinSize = ms;
        if (model.MaxSize is { } mx) win.MaxSize = mx;
        win.Resizable = model.Resizable;
        win.CaptionButtons = model.CaptionButtons;
        win.TopMost = model.TopMost;
        win.Opacity = model.Opacity;
        win.Backdrop = model.Backdrop;
        win.ScreenCapture = model.ScreenCapture;
        win.ApplyTitleBar(model.TitleBar);
    }

    /// <summary>Posición unificada (Phase A — #47): coordenada / preset / snap.</summary>
    private static void ApplyWindowPosition(Windowing.WindowImpl win, WindowModel model)
    {
        if (model.WindowPosition is null) return;
        switch (model.WindowPosition)
        {
            case Share.PositionAt at:
                Win32.SetWindowPos(win.Hwnd, 0, at.X, at.Y, 0, 0,
                    SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
                break;
            case Share.PositionPreset preset:
                ApplyPositionPreset(win, preset.Preset);
                break;
            case Share.PositionSnap snap:
                ApplyPositionSnap(win, snap.Region);
                break;
            default:
                break;
        }
    }

    private static void ApplyPositionPreset(Windowing.WindowImpl win, Share.WindowPositionPreset preset)
    {
        nint mon = Win32.MonitorFromWindow(win.Hwnd, MONITOR.DEFAULTTONEAREST);
        if (mon == 0) mon = Win32.MonitorFromWindow(win.Hwnd, MONITOR.DEFAULTTOPRIMARY);
        if (mon == 0) return;
        MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
        if (!Win32.GetMonitorInfoW(mon, ref mi)) return;
        Win32.GetWindowRect(win.Hwnd, out RECT wr);
        float w = wr.Right - wr.Left, h = wr.Bottom - wr.Top;
        float workW = mi.rcWork.Right - mi.rcWork.Left;
        float workH = mi.rcWork.Bottom - mi.rcWork.Top;
        int x = preset switch
        {
            Share.WindowPositionPreset.Center => mi.rcWork.Left + (int)((workW - w) / 2),
            Share.WindowPositionPreset.TopLeft or Share.WindowPositionPreset.Left or Share.WindowPositionPreset.BottomLeft => mi.rcWork.Left,
            Share.WindowPositionPreset.TopRight or Share.WindowPositionPreset.Right or Share.WindowPositionPreset.BottomRight => mi.rcWork.Right - (int)w,
            _ => mi.rcWork.Left + (int)((workW - w) / 2),
        };
        int y = preset switch
        {
            Share.WindowPositionPreset.Center => mi.rcWork.Top + (int)((workH - h) / 2),
            Share.WindowPositionPreset.TopLeft or Share.WindowPositionPreset.Top or Share.WindowPositionPreset.TopRight => mi.rcWork.Top,
            Share.WindowPositionPreset.BottomLeft or Share.WindowPositionPreset.Bottom or Share.WindowPositionPreset.BottomRight => mi.rcWork.Bottom - (int)h,
            _ => mi.rcWork.Top + (int)((workH - h) / 2),
        };
        Win32.SetWindowPos(win.Hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
    }

    private static void ApplyPositionSnap(Windowing.WindowImpl win, Share.SnapRegion region)
    {
        nint mon = Win32.MonitorFromWindow(win.Hwnd, MONITOR.DEFAULTTONEAREST);
        if (mon == 0) mon = Win32.MonitorFromWindow(win.Hwnd, MONITOR.DEFAULTTOPRIMARY);
        if (mon == 0) return;
        MONITORINFO mi = new() { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFO>() };
        if (!Win32.GetMonitorInfoW(mon, ref mi)) return;
        float workW = mi.rcWork.Right - mi.rcWork.Left;
        float workH = mi.rcWork.Bottom - mi.rcWork.Top;
        // Mitades con división entera determinista: Left/Top están atados, Right/Bottom absorben el resto.
        float hw = workW / 2, hh = workH / 2;
        (int X, int Y, int W, int H) rect = region switch
        {
            Share.SnapRegion.Left => (mi.rcWork.Left, mi.rcWork.Top, (int)hw, (int)workH),
            Share.SnapRegion.Right => (mi.rcWork.Left + (int)hw, mi.rcWork.Top, (int)(workW - hw), (int)workH),
            Share.SnapRegion.Top => (mi.rcWork.Left, mi.rcWork.Top, (int)workW, (int)hh),
            Share.SnapRegion.Bottom => (mi.rcWork.Left, mi.rcWork.Top + (int)hh, (int)workW, (int)(workH - hh)),
            Share.SnapRegion.TopLeft => (mi.rcWork.Left, mi.rcWork.Top, (int)hw, (int)hh),
            Share.SnapRegion.TopRight => (mi.rcWork.Left + (int)hw, mi.rcWork.Top, (int)(workW - hw), (int)hh),
            Share.SnapRegion.BottomLeft => (mi.rcWork.Left, mi.rcWork.Top + (int)hh, (int)hw, (int)(workH - hh)),
            Share.SnapRegion.BottomRight => (mi.rcWork.Left + (int)hw, mi.rcWork.Top + (int)hh, (int)(workW - hw), (int)(workH - hh)),
            _ => (mi.rcWork.Left, mi.rcWork.Top, (int)workW, (int)workH),
        };
        Win32.SetWindowPos(win.Hwnd, 0, rect.X, rect.Y, rect.W, rect.H,
            SWP.NOZORDER | SWP.NOACTIVATE);
    }

    private void SetupSystemTrayIcon(Action<ISystemTray> configure, nint mainHwnd)
    {
        SystemTray.SystemTrayImpl? tray = null;
        var broker = SystemTray.SystemTrayBroker.Create(_router, (trigger, count) => tray?.DispatchTrigger(trigger, count));
        tray = new SystemTray.SystemTrayImpl(broker);
        configure(tray);
        nint hIcon = Core.Windowing.Procedures.LoadAppIcon();
        if (hIcon == 0)
            throw new InvalidOperationException("No hay icono disponible para el SystemTray.");
        if (!broker.AddIcon(hIcon))
            throw new InvalidOperationException("Shell_NotifyIconW(NIM_ADD) falló para el SystemTray.");
        _registry.Attach(EntityKind.Surface, tray, hwnd: 0, tray.Name);
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
        if (window.SurfaceType is not null && typeof(ISystemDialog).IsAssignableFrom(window.SurfaceType))
        {
            RegisterAlternativeDialog(window);
            return;
        }
        if (window.SurfaceType is not null && typeof(IMenu).IsAssignableFrom(window.SurfaceType))
        {
            var surface = new Menus.MenuSurface(window.Content as ContentModel);
            if (!string.IsNullOrEmpty(window.Name))
                surface.Name = window.Name;
            // Dueño de respaldo sin tray: main HWND resuelto en Show() (lazy,
            // igual que diálogos). WireMenuSurfaces() lo sobrescribe con el
            // broker del tray si hay SystemTray declarado.
            surface.OwnerProvider = () => _mainHwnd;
            surface.Registry = _registry;
            surface.EnsureMaterialized();
            _registry.Attach(EntityKind.Surface, surface, hwnd: 0, window.Name);
            _pendingMenuSurfaces.Add(surface);
            WireMenuSurfaces();
            return;
        }
        var hwnd = Core.Windowing.Procedures.CreateAlternativeWindow(_router, 0, window.Title, window.Width, window.Height);
        var win = new Core.Windowing.AlternativeWindowImpl(hwnd, 0, window.Name, window.Title, window.Width, window.Height, window.Position);
        win.BindForRecreation(window, _registry, _router);
        if (window.Background.HasValue)
        {
            var brush = Win32.CreateSolidBrush(window.Background.Value.ToCOLORREF());
            _router.RegisterWindowBackgroundBrush(hwnd, brush);
        }
        _registry.Attach(EntityKind.Window, win, win.Hwnd, win.Name);
        // Fase 2: materializar contenido para AlternativeWindow también
        win.MaterializeContent(window, _registry, _router);
        if (window.Position is WindowPositionOnScreen.Center)
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
                    float winW = wr.Right - wr.Left;
                    float winH = wr.Bottom - wr.Top;
                    int x = mi.rcWork.Left + (int)((mi.rcWork.Right - mi.rcWork.Left - winW) / 2);
                    int y = mi.rcWork.Top + (int)((mi.rcWork.Bottom - mi.rcWork.Top - winH) / 2);
                    Win32.SetWindowPos(hwnd, 0, x, y, 0, 0, SWP.NOZORDER | SWP.NOACTIVATE | SWP.NOSIZE);
                }
            }
        }
        // AlternativeWindow inicia oculta — se muestra explícitamente via GetWindow<IAlternativeWindow>(name).Show()
        // No se llama win.Show() ni win.RaiseLoaded() aquí (Loaded se disparará en el Show manual)
    }

    private void RegisterAlternativeDialog(WindowModel window)
    {
        Dialogs.DialogSurfaceBase surface = window.SurfaceType == typeof(IOpenFileDialog)
            ? new Dialogs.OpenFileDialogImpl()
            : window.SurfaceType == typeof(ISaveFileDialog)
            ? new Dialogs.SaveFileDialogImpl()
            : window.SurfaceType == typeof(ISelectFolderDialog)
            ? new Dialogs.SelectFolderDialogImpl()
            : window.SurfaceType == typeof(ITaskDialog)
            ? new Dialogs.TaskDialogImpl()
            : throw new InvalidOperationException($"Tipo de diálogo del sistema no soportado: {window.SurfaceType}.");

        // Owner resuelto en Show() para no depender del orden de declaración en Layout.
        surface.OwnerProvider = () => _mainHwnd;

        if (window.ConfigureSurface is null)
        {
            if (!string.IsNullOrEmpty(window.Name))
                surface.Name = window.Name;
            if (!string.IsNullOrEmpty(window.Title))
                surface.Title(window.Title);
        }
        else
        {
            window.ConfigureSurface(surface);
        }

        if (string.IsNullOrEmpty(surface.Name))
            throw new InvalidOperationException("Los diálogos del sistema requieren Name(...).");
        _registry.Attach(EntityKind.Surface, surface, hwnd: 0, surface.Name);
    }
}
