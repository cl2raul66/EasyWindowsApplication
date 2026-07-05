using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Frontend;

internal sealed class ApplicationContext :
    IApplicationLayoutPhase,
    IApplicationBehaviorPhase,
    IApplicationInitializationPhase
{
    internal IResourcesDictionary Resources { get; } = new ResourcesDictionaryImpl();
    internal ILayoutBuilder LayoutBuilder { get; } = new LayoutBuilderImpl();
    internal IBehaviorBuilder BehaviorBuilder { get; } = new BehaviorBuilderImpl();

    public IApplicationBehaviorPhase Layout() => this;

    public IApplicationBehaviorPhase Layout(Action<ILayoutBuilder> configure)
    {
        configure(this.LayoutBuilder);
        return this;
    }

    public IApplicationInitializationPhase Behavior() => this;

    public IApplicationInitializationPhase Behavior(Action<IBehaviorBuilder> configure)
    {
        configure(this.BehaviorBuilder);
        return this;
    }

    public void Initialize()
    {
        // El CoreModule toma el control y arranca Win32
        MasterRouter.StartApplication(this);
    }
}
