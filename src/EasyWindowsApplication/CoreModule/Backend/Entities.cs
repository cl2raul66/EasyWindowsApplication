using System.Runtime.InteropServices;

namespace EasyWindowsApplication.CoreModule.Backend;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEXW
{
    public uint cbSize;
    public uint style;
    public nint lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public nint hInstance;
    public nint hIcon;
    public nint hCursor;
    public nint hbrBackground;
    public nint lpszMenuName;
    public nint lpszClassName;
    public nint hIconSm;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public nint hwnd;
    public uint message;
    public nint wParam;
    public nint lParam;
    public uint time;
    public POINT pt;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int X;
    public int Y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

[StructLayout(LayoutKind.Sequential)]
internal struct NMHDR
{
    public nint hwndFrom;
    public nint idFrom;
    public uint code;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct NMLVDISPINFOW
{
    public NMHDR hdr;
    public int iItem;
    public int iSubItem;
    public uint mask;
    public int state;
    public int stateMask;
    public nint pszText;
    public int cchTextMax;
    public int iImage;
    public nint lParam;
}

[StructLayout(LayoutKind.Sequential)]
internal struct LVITEMW
{
    public uint mask;
    public int iItem;
    public int iSubItem;
    public uint state;
    public uint stateMask;
    public nint pszText;
    public int cchTextMax;
    public int iImage;
    public nint lParam;
}

[StructLayout(LayoutKind.Sequential)]
internal struct LVCOLUMNW
{
    public uint mask;
    public int fmt;
    public int cx;
    public nint pszText;
    public int cchTextMax;
    public int iSubItem;
    public int iImage;
    public int iOrder;
}

[StructLayout(LayoutKind.Sequential)]
internal struct CREATESTRUCTW
{
    public nint lpCreateParams;
    public nint hInstance;
    public nint hMenu;
    public nint hwndParent;
    public int cy;
    public int cx;
    public int y;
    public int x;
    public int style;
    public nint lpszName;
    public nint lpszClass;
    public uint dwExStyle;
}
