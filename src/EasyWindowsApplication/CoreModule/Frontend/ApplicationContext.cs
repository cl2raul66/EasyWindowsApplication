using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class ApplicationContext :
    IApplicationLayoutPhase,
    IApplicationBehaviorPhase,
    IApplicationInitializationPhase
{
    internal IResourcesDictionary ResourcesDict { get; } = new ResourcesDictionaryImpl();
    internal ILayoutBuilder LayoutBuilder { get; } = new LayoutBuilderImpl();
    internal IBehaviorBuilder BehaviorBuilder { get; } = new BehaviorBuilderImpl();

    private readonly HandleRegistry _registry = new();

    public IApplicationBehaviorPhase Layout() => this;

    public IApplicationBehaviorPhase Layout(Action<ILayoutBuilder> configure)
    {
        var router = new MasterRouter(_registry);
        Procedures.SetRouter(router);
        ControlAccess.Initialize(_registry);

        var layoutImpl = (LayoutBuilderImpl)LayoutBuilder;
        layoutImpl.Router = router;
        layoutImpl.Registry = _registry;

        configure(layoutImpl);

        return this;
    }

    public IApplicationInitializationPhase Behavior() => this;

    public IApplicationInitializationPhase Behavior(Action<IBehaviorBuilder> configure)
    {
        var behaviorImpl = (BehaviorBuilderImpl)BehaviorBuilder;
        behaviorImpl.Registry = _registry;

        configure(behaviorImpl);
        behaviorImpl.ApplyPending(_registry);
        return this;
    }

    public void Initialize()
    {
        var behaviorImpl = (BehaviorBuilderImpl)BehaviorBuilder;

        if (behaviorImpl.Win32Configurator is not null)
        {
            var win32State = new Win32StateImpl(_registry);
            behaviorImpl.Win32Configurator(win32State);
        }

        Procedures.RunMessageLoop();
    }
}

internal sealed class Win32StateImpl : IWin32State
{
    private readonly HandleRegistry _registry;

    internal Win32StateImpl(HandleRegistry registry) => _registry = registry;

    public T Get<T>(string name) where T : ControlBase<T>
    {
        var control = _registry.GetByName(name)
            ?? throw new InvalidOperationException($"Control '{name}' not found in the layout.");
        return (T)control;
    }
}
