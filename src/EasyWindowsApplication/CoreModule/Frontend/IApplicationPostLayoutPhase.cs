namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IApplicationPostLayoutPhase
{
    IApplicationPostBehaviorPhase Behavior();
    IApplicationPostBehaviorPhase Behavior(Action<IBehaviorBuilder> configure);
    void Initialize();
}