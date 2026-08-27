namespace EasyWindowsApplication.Share;

public sealed class WindowMovedEventArgs : EventArgs
{
    public int X { get; }
    public int Y { get; }
    public WindowMovedEventArgs(int x, int y) { X = x; Y = y; }
}
