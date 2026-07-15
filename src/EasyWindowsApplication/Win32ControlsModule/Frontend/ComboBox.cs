using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ComboBox : ControlBase<ComboBox>
{
    public int AddItem(string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            return (int)ControlProcedures.SendMessage(Hwnd, 0x0143 /* CB_ADDSTRING */, 0, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void RemoveItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0144 /* CB_DELETESTRING */, (nint)index, 0);
    }

    public void ClearItems()
    {
        ControlProcedures.SendMessage(Hwnd, 0x014B /* CB_RESETCONTENT */, 0, 0);
    }

    public int GetCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x0146 /* CB_GETCOUNT */, 0, 0);
    }

    public int GetSelectedIndex()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x0147 /* CB_GETCURSEL */, 0, 0);
    }

    public void SetSelectedIndex(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x014E /* CB_SETCURSEL */, (nint)index, 0);
    }

    public string GetItemText(int index)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, 0x0149 /* CB_GETLBTEXTLEN */, (nint)index, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, 0x0148 /* CB_GETLBTEXT */, (nint)index, buffer);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }
}
