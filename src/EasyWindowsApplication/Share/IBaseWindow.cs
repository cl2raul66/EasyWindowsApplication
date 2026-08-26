namespace EasyWindowsApplication.Share;

public interface IBaseWindow
{
    nint Hwnd { get; }
    string Name { get; }
    void Show();
    void Hide();
    void Close();
}
