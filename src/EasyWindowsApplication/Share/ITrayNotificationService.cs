namespace EasyWindowsApplication.Share;

public interface ITrayNotificationService
{
    ISystemTray Notify(string title, string message);
    ISystemTray DismissNotification();
}
