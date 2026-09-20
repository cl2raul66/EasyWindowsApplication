using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Share;

public interface ISystemTray : IViewSurface, ISystemTrayNotifications
{
    ISystemTray OnInputWithSpatialPosition<TTrigger>(Action handler)
        where TTrigger : ISpatialPositionTrigger;
    ISystemTray OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCount;
    ISystemTray OnInputWithoutSpatialPosition<TTrigger>(Action handler)
        where TTrigger : WithoutSpatialPositionTrigger;
}
