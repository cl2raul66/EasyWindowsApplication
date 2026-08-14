namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IApplicationPostLayoutPhase
{
    IApplicationPostBehaviorPhase Behavior(Action<IBehaviorBuilder> configure);
    void Initialize();
}