namespace EasyWindowsApplication.Share;

public interface IBaseWindow
{
    nint Hwnd { get; }
    string Name { get; }
    void Show();
    void Hide();
    void Close();

    event EventHandler? Loaded;
    event EventHandler<CancelEventArgs>? Closing;
    event EventHandler? Closed;
    event EventHandler? Activated;
    event EventHandler? Deactivated;
}
