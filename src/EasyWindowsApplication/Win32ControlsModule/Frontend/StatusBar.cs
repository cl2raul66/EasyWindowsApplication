using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class StatusBar : ControlBase<StatusBar>
{
    public void SetParts(int[] widths)
    {
        ControlProcedures.SendMessage(Hwnd, SB.PARTS, (nint)widths.Length, 0);
        for (int i = 0; i < widths.Length; i++)
        {
            // Use SB_SETTEXT with part index
            nint lParam = 0;
            ControlProcedures.SendMessage(Hwnd, 0x040B /* SB_SETTEXTW */, (nint)i, lParam);
        }
    }

    public void SetText(int part, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x040B /* SB_SETTEXTW */, (nint)part, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public string GetText(int part)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, 0x040C /* SB_GETTEXTLENGTHW */, (nint)part, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x040D /* SB_GETTEXTW */, (nint)part, buffer);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void SetSimple(bool simple)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0418 /* SB_SIMPLE */, simple ? 1 : 0, 0);
    }

    public void SetTipText(int part, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x0411 /* SB_SETTIPTEXTW */, (nint)part, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }
}
