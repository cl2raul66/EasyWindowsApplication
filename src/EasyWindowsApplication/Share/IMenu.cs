namespace EasyWindowsApplication.Share;

public interface IMenu : IViewSurface
{
    void Show();
    void ShowAtCursor();
    void ShowAtIcon();
    void Hide();
    void Close();
}
