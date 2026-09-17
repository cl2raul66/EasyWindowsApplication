namespace EasyWindowsApplication.Share;

public interface IBehaviorBuilder
{
    IAppBehavior WindowsApplication { get; }
    ISystemTray SystemTray { get; }
}
