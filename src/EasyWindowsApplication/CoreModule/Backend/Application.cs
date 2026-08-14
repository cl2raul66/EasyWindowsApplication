using EasyWindowsApplication.CoreModule.Frontend;
using EasyWindowsApplication.LayoutModule.Backend;
using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Infrastructure;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Backend;

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
        _router = new MasterRouter(_registry);

        foreach (var window in _windows)
        {
            if (window.IsAlternative)
                RegisterAlternative(window);
            else
                RegisterMain(window);
        }

        if (BehaviorBuilder.Win32Configurator is not null)
            BehaviorBuilder.Win32Configurator(new Win32StateImpl(_registry));

        Procedures.RunMessageLoop();
    }

    private void RegisterMain(WindowModel window)
    {
        var hwnd = WindowingModule.Backend.Procedures.CreateMainWindow(_router, window.Title, window.Width, window.Height);

        var win = new WindowingModule.Backend.WindowImpl(
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
        _registry.RegisterWindow(new WindowingModule.Backend.AlternativeWindowImpl(
            0, 0, window.Name, window.Title, window.Width, window.Height, window.Position));
    }
}

internal sealed class Win32StateImpl : IWin32State
{
    private readonly HandleRegistry _registry;
    internal Win32StateImpl(HandleRegistry registry) => _registry = registry;

    public T Get<T>(string name) where T : View<T>
    {
        var control = _registry.GetByName(name)
            ?? throw new InvalidOperationException($"Control '{name}' not found in the layout.");
        return (T)control;
    }
}