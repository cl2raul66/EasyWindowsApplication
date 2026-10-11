namespace EasyWindowsApplication.Share;

public interface IBaseWindow
{
    nint Hwnd { get; }
    string Name { get; }
    void Show();
    void Hide();

    // Behavior-first lifecycle (aligns with IAppBehavior dialect)
    void OnLoaded(Action handler);
    void OnActivated(Action handler);
    void OnDeactivated(Action handler);

    // LEGACY: retained for compatibility (not [Obsolete] yet)
    event EventHandler? Loaded;
    event EventHandler? Activated;
    event EventHandler? Deactivated;
}
