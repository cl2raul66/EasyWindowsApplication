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
            return (int)ControlProcedures.SendMessage(Hwnd, LB.ADDSTRING, 0, textPtr);
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
            return (int)ControlProcedures.SendMessage(Hwnd, LB.INSERTSTRING, (nint)index, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void RemoveItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, LB.DELETESTRING, (nint)index, 0);
    }

    public void ClearItems()
    {
        ControlProcedures.SendMessage(Hwnd, LB.RESETCONTENT, 0, 0);
    }

    public int GetCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LB.GETCOUNT, 0, 0);
    }

    public int GetSelectedIndex()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LB.GETCURSEL, 0, 0);
    }

    public void SetSelectedIndex(int index)
    {
        ControlProcedures.SendMessage(Hwnd, LB.SETCURSEL, (nint)index, 0);
    }

    public string GetItemText(int index)
    {
        int length = (int)ControlProcedures.SendMessage(Hwnd, LB.GETTEXTLEN, (nint)index, 0);
        if (length <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, LB.GETTEXT, (nint)index, buffer);
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
            return (int)ControlProcedures.SendMessage(Hwnd, LB.FINDSTRING, (nint)startIndex, textPtr);
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
            return (int)ControlProcedures.SendMessage(Hwnd, LB.FINDSTRINGEXACT, (nint)startIndex, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public bool IsSelected(int index)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LB.GETSEL, (nint)index, 0) > 0;
    }

    public int GetSelCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LB.GETSELCOUNT, 0, 0);
    }

    public int[] GetSelItems()
    {
        int count = GetSelCount();
        if (count <= 0) return [];

        nint buffer = Marshal.AllocHGlobal(count * 4);
        try
        {
            ControlProcedures.SendMessage(Hwnd, LB.GETSELITEMS, (nint)count, buffer);
            int[] items = new int[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = Marshal.ReadInt32(buffer, i * 4);
            }
            return items;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public int TopIndex
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, LB.GETTOPINDEX, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, LB.SETTOPINDEX, (nint)value, 0);
    }

    public int HorizontalExtent
    {
        set => ControlProcedures.SendMessage(Hwnd, LB.SETHORIZONTALEXTENT, (nint)value, 0);
    }

    public int GetItemHeight(int index = -1)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x01A1 /* LB_GETITEMHEIGHT */, (nint)index, 0);
    }

    public void SetItemHeight(int index, int height)
    {
        ControlProcedures.SendMessage(Hwnd, 0x01A0 /* LB_SETITEMHEIGHT */, (nint)index, (nint)height);
    }
}
