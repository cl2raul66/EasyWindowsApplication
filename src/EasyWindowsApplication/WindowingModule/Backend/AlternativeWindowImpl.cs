using EasyWindowsApplication.Share;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.WindowingModule.Backend;

internal sealed class AlternativeWindowImpl : IAlternativeWindow
{
    public nint Hwnd { get; }
    public string Name { get; }
    public nint OwnerHwnd { get; }
    public string Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public WindowPositionOnScreen PositionMode { get; set; }

    internal AlternativeWindowImpl(nint hwnd, nint ownerHwnd, string name, string title, int width, int height, WindowPositionOnScreen position)
    {
        Hwnd = hwnd;
        OwnerHwnd = ownerHwnd;
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
