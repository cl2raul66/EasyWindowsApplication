namespace EasyWindowsApplication.Share;

public interface IAppBehavior
{
    IAppBehavior OnLaunched(Action handler);
    IAppBehavior OnLaunched(Action<IAppBehavior> handler);
    IAppBehavior OnTerminating(Action<CancelEventArgs> handler);
    IAppBehavior OnTerminated(Action handler);

    IAppBehavior TaskbarButtonVisibility(bool visible);
}