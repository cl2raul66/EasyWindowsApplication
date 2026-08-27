
namespace EasyWindowsApplication.Share;

public interface IWindow : IBaseWindow
{
    string Title { get; set; }
    float Width { get; set; }
    float Height { get; set; }
    WindowPositionOnScreen PositionMode { get; set; }

    event EventHandler<WindowResizingEventArgs>? Resizing;
    event EventHandler<WindowResizedEventArgs>? Resized;
    event EventHandler<WindowMovedEventArgs>? Moved;

    void Center();
    void Maximize();
    void Minimize();
    void Restore();
    void Focus();

    (int X, int Y) ScrollOffset { get; }
    void ScrollTo(int x, int y);
}
