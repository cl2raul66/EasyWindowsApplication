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
            return (int)ControlProcedures.SendMessage(Hwnd, CB.ADDSTRING, 0, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public int InsertItem(int index, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            return (int)ControlProcedures.SendMessage(Hwnd, CB.INSERTSTRING, (nint)index, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void RemoveItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, CB.DELETESTRING, (nint)index, 0);
    }

    public void ClearItems()
    {
        ControlProcedures.SendMessage(Hwnd, CB.RESETCONTENT, 0, 0);
    }

    public int GetCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, CB.GETCOUNT, 0, 0);
    }

    public int GetSelectedIndex()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, CB.GETCURSEL, 0, 0);
    }

    public void SetSelectedIndex(int index)
    {
        ControlProcedures.SendMessage(Hwnd, CB.SETCURSEL, (nint)index, 0);
    }

    public string GetItemText(int index)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, CB.GETLBTEXTLEN, (nint)index, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, CB.GETLBTEXT, (nint)index, buffer);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public int FindString(string text, int startIndex = -1)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            return (int)ControlProcedures.SendMessage(Hwnd, CB.FINDSTRING, (nint)startIndex, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public int FindStringExact(string text, int startIndex = -1)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            return (int)ControlProcedures.SendMessage(Hwnd, CB.FINDSTRINGEXACT, (nint)startIndex, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }

    public void ShowDropdown(bool show)
    {
        ControlProcedures.SendMessage(Hwnd, CB.SHOWDROPDOWN, show ? 1 : 0, 0);
    }

    public bool IsDroppedDown
    {
        get => ControlProcedures.SendMessage(Hwnd, CB.GETDROPPEDSTATE, 0, 0) != 0;
    }

    public int GetEditSelStart()
    {
        nint result = ControlProcedures.SendMessage(Hwnd, CB.GETEDITSEL, 0, 0);
        return (int)(result & 0xFFFF);
    }

    public int GetEditSelEnd()
    {
        nint result = ControlProcedures.SendMessage(Hwnd, CB.GETEDITSEL, 0, 0);
        return (int)((result >> 16) & 0xFFFF);
    }

    public void SetEditSel(int start, int end)
    {
        ControlProcedures.SendMessage(Hwnd, CB.SETEDITSEL, 0, (nint)((start & 0xFFFF) | ((end & 0xFFFF) << 16)));
    }

    public bool ExtendedUI
    {
        get => ControlProcedures.SendMessage(Hwnd, CB.GETEXTENDEDUI, 0, 0) != 0;
        set => ControlProcedures.SendMessage(Hwnd, CB.SETEXTENDEDUI, value ? 1 : 0, 0);
    }

    public int TopIndex
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, CB.GETTOPINDEX, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, CB.SETTOPINDEX, (nint)value, 0);
    }

    public int DroppedWidth
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, CB.GETDROPPEDWIDTH, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, CB.SETDROPPEDWIDTH, (nint)value, 0);
    }

    public int GetItemHeight(int index = -1)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, CB.GETITEMHEIGHT, (nint)index, 0);
    }

    public void SetItemHeight(int index, int height)
    {
        ControlProcedures.SendMessage(Hwnd, CB.SETITEMHEIGHT, (nint)index, (nint)height);
    }

    public int LimitText
    {
        set => ControlProcedures.SendMessage(Hwnd, CB.LIMITTEXT, (nint)value, 0);
    }
}
