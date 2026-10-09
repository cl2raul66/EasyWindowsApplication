using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core;

[StructLayout(LayoutKind.Sequential)]
internal struct SIZE
{
    public int cx;
    public int cy;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ABC
{
    public int abcA;
    public uint abcB;
    public int abcC;
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
internal struct SCROLLINFO
{
    public uint cbSize;
    public uint fMask;
    public int nMin;
    public int nMax;
    public uint nPage;
    public int nPos;
    public int nTrackPos;
}

[StructLayout(LayoutKind.Sequential)]
internal struct INITCOMMONCONTROLSEX
{
    public uint dwSize;
    public uint dwICC;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MONITORINFO
{
    public uint cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MARGINS
{
    public int cxLeftWidth;
    public int cxRightWidth;
    public int cyTopHeight;
    public int cyBottomHeight;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MINMAXINFO
{
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
}

[StructLayout(LayoutKind.Sequential)]
internal struct CWPSTRUCT
{
    public nint lParam;
    public nint wParam;
    public uint message;
    public nint hwnd;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DRAWITEMSTRUCT
{
    public uint CtlType;
    public uint CtlID;
    public uint itemID;
    public uint itemAction;
    public uint itemState;
    public nint hwndItem;
    public nint hDC;
    public RECT rcItem;
    public nint itemData;
}
