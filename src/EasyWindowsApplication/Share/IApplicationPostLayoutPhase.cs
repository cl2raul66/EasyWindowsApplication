namespace EasyWindowsApplication.Share;

public interface IApplicationPostLayoutPhase
{
    IApplicationPostBehaviorPhase Behavior(Action<IBehaviorBuilder> configure);
    void Initialize();
}