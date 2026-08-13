using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct LVITEMW
{
    internal uint mask;
    internal int iItem;
    internal int iSubItem;
    internal uint state;
    internal uint stateMask;
    internal nint pszText;
    internal int cchTextMax;
    internal int iImage;
    internal nint lParam;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct LVCOLUMNW
{
    internal uint mask;
    internal int fmt;
    internal int cx;
    internal nint pszText;
    internal int cchTextMax;
    internal int iSubItem;
    internal int iImage;
    internal int iOrder;
}
