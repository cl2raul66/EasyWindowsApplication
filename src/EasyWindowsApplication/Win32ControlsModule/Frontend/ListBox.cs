using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ListBox : ControlBase<ListBox>
{
    public int AddItem(string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            return (int)ControlProcedures.SendMessage(Hwnd, 0x0180 /* LB_ADDSTRING */, 0, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void RemoveItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0182 /* LB_DELETESTRING */, (nint)index, 0);
    }

    public void ClearItems()
    {
        ControlProcedures.SendMessage(Hwnd, 0x0184 /* LB_RESETCONTENT */, 0, 0);
    }

    public int GetCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x018B /* LB_GETCOUNT */, 0, 0);
    }

    public int GetSelectedIndex()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x0188 /* LB_GETCURSEL */, 0, 0);
    }

    public void SetSelectedIndex(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0186 /* LB_SETCURSEL */, (nint)index, 0);
    }

    public string GetItemText(int index)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, 0x018A /* LB_GETTEXTLEN */, (nint)index, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x0189 /* LB_GETTEXT */, (nint)index, buffer);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
