namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IApplicationBehaviorPhase : IApplicationInitializationPhase
{
    IApplicationInitializationPhase Behavior();
    IApplicationInitializationPhase Behavior(Action<IBehaviorBuilder> configure);
}
