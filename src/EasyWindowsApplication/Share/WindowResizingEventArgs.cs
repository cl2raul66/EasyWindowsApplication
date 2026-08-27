namespace EasyWindowsApplication.Share;

public sealed class WindowResizingEventArgs : EventArgs
{
    public int Width { get; }
    public int Height { get; }
    public WindowResizingEventArgs(int width, int height) { Width = width; Height = height; }
}
