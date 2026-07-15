using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ScrollBar : ControlBase<ScrollBar>
{
    public int Position
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, SBM.GETPOS, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, SBM.SETPOS, (nint)value, 1);
    }

    public (int min, int max) Range
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, SBM.SETRANGE, (nint)value.min, (nint)value.max);
        }
    }

    public (int min, int max) GetRange()
    {
        nint minPtr = Marshal.AllocHGlobal(4);
        nint maxPtr = Marshal.AllocHGlobal(4);
        try
        {
            ControlProcedures.SendMessage(Hwnd, SBM.GETRANGE, minPtr, maxPtr);
            int min = Marshal.ReadInt32(minPtr);
            int max = Marshal.ReadInt32(maxPtr);
            return (min, max);
        }
        finally
        {
            Marshal.FreeHGlobal(minPtr);
            Marshal.FreeHGlobal(maxPtr);
        }
    }

    public (int min, int max) Range32
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, SBM.SETRANGEREDRAW, (nint)value.min, (nint)value.max);
        }
    }

    public void EnableArrows(int flags)
    {
        ControlProcedures.SendMessage(Hwnd, SBM.ENABLE_ARROWS, (nint)flags, 0);
    }

    public bool IsEnabled
    {
        get => true; // ScrollBar is always enabled by default
    }

    public void SetScrollInfo(int min, int max, int page = 0, int pos = 0)
    {
        var scrollInfo = new SCROLLINFO
        {
            cbSize = (uint)Marshal.SizeOf<SCROLLINFO>(),
            fMask = 0x0011, // SIF_RANGE | SIF_PAGE | SIF_POS
            nMin = min,
            nMax = max,
            nPage = (uint)page,
            nPos = pos,
        };

        nint structPtr = Marshal.AllocHGlobal(Marshal.SizeOf<SCROLLINFO>());
        try
        {
            Marshal.StructureToPtr(scrollInfo, structPtr, false);
            ControlProcedures.SendMessage(Hwnd, SBM.SETSCROLLINFO, 1, structPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(structPtr);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct SCROLLINFO
{
    internal uint cbSize;
    internal uint fMask;
    internal int nMin;
    internal int nMax;
    internal uint nPage;
    internal int nPos;
    internal int nTrackPos;
}
