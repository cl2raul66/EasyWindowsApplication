using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class StatusBar : ControlBase<StatusBar>
{
    public void SetParts(int[] widths)
    {
        if (widths.Length == 0) return;
        ControlProcedures.SendMessage(Hwnd, SB.PARTS, (nint)widths.Length, 0);
    }

    public void SetText(int part, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            ControlProcedures.SendMessage(Hwnd, SB.SETTEXTW, (nint)part, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public string GetText(int part)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, SB.GETTEXTLENGTHW, (nint)part, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, SB.GETTEXTW, (nint)part, buffer);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void SetSimple(bool simple)
    {
        ControlProcedures.SendMessage(Hwnd, SB.SETSimple, simple ? 1 : 0, 0);
    }

    public bool IsSimple
    {
        get => ControlProcedures.SendMessage(Hwnd, SB.ISSIMPLE, 0, 0) != 0;
    }

    public void SetTipText(int part, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            ControlProcedures.SendMessage(Hwnd, SB.SETTIPTEXTW, (nint)part, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public string GetTipText(int part)
    {
        nint buffer = Marshal.AllocHGlobal(256 * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x0413 /* SB_GETTIPTEXTW */, (nint)part, buffer);
            return Marshal.PtrToStringUni(buffer) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void SetIcon(int part, nint hIcon)
    {
        ControlProcedures.SendMessage(Hwnd, SB.SETICON, (nint)part, hIcon);
    }

    public int PartCount
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, SB.GETPARTS, 0, 0);
    }

    public (int horizontal, int vertical, int divider) GetBorders()
    {
        nint buffer = Marshal.AllocHGlobal(12); // 3 ints
        try
        {
            ControlProcedures.SendMessage(Hwnd, SB.GETBORDERS, 0, buffer);
            int horizontal = Marshal.ReadInt32(buffer, 0);
            int vertical = Marshal.ReadInt32(buffer, 4);
            int divider = Marshal.ReadInt32(buffer, 8);
            return (horizontal, vertical, divider);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void SetMinHeight(int minHeight)
    {
        ControlProcedures.SendMessage(Hwnd, SB.SETMINHEIGHT, (nint)minHeight, 0);
    }

    public (int left, int top, int right, int bottom) GetRect(int part)
    {
        nint rectPtr = Marshal.AllocHGlobal(16);
        try
        {
            Marshal.WriteInt32(rectPtr, 0, 0);
            Marshal.WriteInt32(rectPtr, 4, 0);
            Marshal.WriteInt32(rectPtr, 8, 0);
            Marshal.WriteInt32(rectPtr, 12, 0);
            ControlProcedures.SendMessage(Hwnd, SB.GETRECT, (nint)part, rectPtr);
            int left = Marshal.ReadInt32(rectPtr, 0);
            int top = Marshal.ReadInt32(rectPtr, 4);
            int right = Marshal.ReadInt32(rectPtr, 8);
            int bottom = Marshal.ReadInt32(rectPtr, 12);
            return (left, top, right, bottom);
        }
        finally
        {
            Marshal.FreeHGlobal(rectPtr);
        }
    }
}
