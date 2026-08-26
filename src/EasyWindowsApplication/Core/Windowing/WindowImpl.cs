using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Windowing;

internal sealed class WindowImpl : IWindow
{
    public nint Hwnd { get; }
    public string Name { get; }
    public string Title { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public WindowPositionOnScreen PositionMode { get; set; }

    internal WindowImpl(nint hwnd, string name, string title, float width, float height, WindowPositionOnScreen position)
    {
        Hwnd = hwnd;
        Name = name;
        Title = title;
        Width = width;
        Height = height;
        PositionMode = position;
    }

    public void Show() => Win32.ShowWindow(Hwnd, 5);
    public void Hide() => Win32.ShowWindow(Hwnd, 0);
    public void Close() => Win32.DestroyWindow(Hwnd);
}
