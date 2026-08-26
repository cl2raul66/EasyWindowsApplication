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
        BehaviorBuilder.Registry = _registry;
        ControlAccess.SetController(BehaviorBuilder);
        configure(BehaviorBuilder);
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
        ControlActivatorRegistry.EnsureInitialized();
        _router = new MasterRouter(_registry);

        foreach (var window in _windows)
        {
            if (window.IsAlternative)
                RegisterAlternative(window);
            else
                RegisterMain(window);
        }

        Procedures.RunMessageLoop();
    }

    private void RegisterMain(WindowModel window)
    {
        var hwnd = Core.Windowing.Procedures.CreateMainWindow(_router, window.Title, window.Width, window.Height);

        var win = new Core.Windowing.WindowImpl(
            hwnd, window.Name, window.Title, window.Width, window.Height, window.Position);

        if (window.Background.HasValue)
        {
            var brush = Win32.CreateSolidBrush(window.Background.Value.ToCOLORREF());
            _router.RegisterWindowBackgroundBrush(hwnd, brush);
        }

        _registry.RegisterWindow(win);
        win.Show();
    }

    private void RegisterAlternative(WindowModel window)
    {
        var hwnd = Core.Windowing.Procedures.CreateAlternativeWindow(_router, 0, window.Title, window.Width, window.Height);
        var win = new Core.Windowing.AlternativeWindowImpl(hwnd, 0, window.Name, window.Title, window.Width, window.Height, window.Position);
        if (window.Background.HasValue)
        {
            var brush = Win32.CreateSolidBrush(window.Background.Value.ToCOLORREF());
            _router.RegisterWindowBackgroundBrush(hwnd, brush);
        }
        _registry.RegisterWindow(win);
        win.Show();
    }
}
