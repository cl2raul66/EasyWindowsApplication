namespace EasyWindowsApplication.Share;

public sealed class WindowResizingEventArgs : EventArgs
{
    public float Width { get; }
    public float Height { get; }
    public WindowResizingEventArgs(float width, float height) { Width = width; Height = height; }
}
