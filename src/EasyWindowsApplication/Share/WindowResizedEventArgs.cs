namespace EasyWindowsApplication.Share;

public sealed class WindowResizedEventArgs : EventArgs
{
    public int Width { get; }
    public int Height { get; }
    public WindowResizedEventArgs(int width, int height) { Width = width; Height = height; }
}
