namespace EasyWindowsApplication.Share;

public interface IWindow : IBaseWindow, IInputSurface
{
    string Title { get; set; }
    float Width { get; set; }
    float Height { get; set; }
    WindowPositionOnScreen PositionMode { get; set; }

    // Behavior-first lifecycle & geometry (#54)
    void OnResizing(Action<WindowResizingEventArgs> handler);
    void OnResized(Action<WindowResizedEventArgs> handler);
    void OnMoved(Action<WindowMovedEventArgs> handler);

    // LEGACY (eventos C# internos — no usar en Behavior)
    event EventHandler<WindowResizingEventArgs>? Resizing;
    event EventHandler<WindowResizedEventArgs>? Resized;
    event EventHandler<WindowMovedEventArgs>? Moved;

    void Center();
    void Maximize();
    void Minimize();
    void Restore();
    void Focus();
    void Visibility(bool visible);

    // Phase A: state & appearance
    WindowState State { get; set; }
    void EnterFullScreen();
    void ExitFullScreen();
    bool TopMost { get; set; }
    double Opacity { get; set; }
    WindowBackdrop Backdrop { get; set; }
    ScreenCaptureMode ScreenCapture { get; set; }

    // Phase B: sizing constraints
    (int MinWidth, int MinHeight) MinSize { get; set; }
    (int MaxWidth, int MaxHeight) MaxSize { get; set; }
    bool Resizable { get; set; }
    CaptionButtons CaptionButtons { get; set; }

    // Phase D: Z-order
    void BringToFront();
    void SendToBack();

    (int X, int Y) ScrollOffset { get; }
    void ScrollTo(int x, int y);
}
