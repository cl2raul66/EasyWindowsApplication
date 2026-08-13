using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ReBar : ControlBase<ReBar>
{
    public int AddBand(nint hwndChild, string text, int width = -1)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var bandInfo = new REBARBANDINFOW
            {
                cbSize = (uint)Marshal.SizeOf<REBARBANDINFOW>(),
                fMask = 0x0011, // RBBIM_STYLE | RBBIM_TEXT | RBBIM_CHILD
                fStyle = 0x0000, // RBBS_BREAK
                hwndChild = hwndChild,
                lpText = textPtr,
                cx = width,
                cyChild = 0,
                cyMinChild = 0,
                cxMinChild = 0,
            };

            nint bandPtr = Marshal.AllocHGlobal(Marshal.SizeOf<REBARBANDINFOW>());
            try
            {
                Marshal.StructureToPtr(bandInfo, bandPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, RB.INSERTBANDW, (nint)(-1), bandPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(bandPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void DeleteBand(int index)
    {
        ControlProcedures.SendMessage(Hwnd, RB.DELETEBAND, (nint)index, 0);
    }

    public int BandCount
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, RB.GETBANDCOUNT, 0, 0);
    }

    public int RowCount
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, RB.GETROWCOUNT, 0, 0);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct REBARBANDINFOW
{
    internal uint cbSize;
    internal uint fMask;
    internal uint fStyle;
    internal int clrFore;
    internal int clrBack;
    internal nint lpText;
    internal int cch;
    internal int iImage;
    internal nint hwndChild;
    internal int cxMinChild;
    internal int cyMinChild;
    internal int cx;
    internal int hbmBack;
    internal int wID;
    internal int cyChild;
    internal int cyMaxChild;
    internal int cyIntegral;
    internal int cxIdeal;
    internal nint lParam;
    internal int cxHeader;
}
