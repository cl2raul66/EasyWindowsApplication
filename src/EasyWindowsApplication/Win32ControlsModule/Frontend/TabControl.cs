using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class TabControl : ControlBase<TabControl>
{
    public int AddTab(string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new TCITEMW
            {
                mask = 0x0001, // TCIF_TEXT
                pszText = textPtr,
                cchTextMax = text.Length,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TCITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, TCM.INSERTITEMW, (nint)GetTabCount(), itemPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(itemPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void DeleteAllTabs()
    {
        ControlProcedures.SendMessage(Hwnd, TCM.DELETEALLITEMS, 0, 0);
    }

    public int GetTabCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x1304 /* TCM_GETITEMCOUNT */, 0, 0);
    }

    public int SelectedIndex
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, TCM.GETCURSEL, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, TCM.SETCURSEL, (nint)value, 0);
    }

    public string GetTabText(int index)
    {
        var item = new TCITEMW
        {
            mask = 0x0001, // TCIF_TEXT
        };

        nint buffer = Marshal.AllocHGlobal(256 * 2);
        try
        {
            item.pszText = buffer;
            item.cchTextMax = 256;

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TCITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                ControlProcedures.SendMessage(Hwnd, TCM.GETITEMW, (nint)index, itemPtr);
                return Marshal.PtrToStringUni(buffer) ?? string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(itemPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void HighlightFirstItem()
    {
        ControlProcedures.SendMessage(Hwnd, TCM.HIGHLIGHTFIRSTITEM, 0, 0);
    }

    public void DeleteTab(int index)
    {
        ControlProcedures.SendMessage(Hwnd, TCM.DELETEALLITEMS, (nint)index, 0);
    }

    public int GetItemCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, TCM.GETITEMCOUNT, 0, 0);
    }

    public (int left, int top, int right, int bottom) GetItemRect(int index)
    {
        nint rectPtr = Marshal.AllocHGlobal(16); // RECT is 16 bytes
        try
        {
            Marshal.WriteInt32(rectPtr, 0, 0);
            Marshal.WriteInt32(rectPtr, 4, 0);
            Marshal.WriteInt32(rectPtr, 8, 0);
            Marshal.WriteInt32(rectPtr, 12, 0);
            ControlProcedures.SendMessage(Hwnd, TCM.GETITEMRECT, (nint)index, rectPtr);
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

    public void SetMinTabWidth(int width)
    {
        ControlProcedures.SendMessage(Hwnd, TCM.SETMINTABWIDTH, 0, (nint)width);
    }

    public void DeselectAll(bool excludeFocus = true)
    {
        ControlProcedures.SendMessage(Hwnd, TCM.DeselectAll, excludeFocus ? 1 : 0, 0);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct TCITEMW
{
    internal uint mask;
    internal uint dwState;
    internal uint dwStateMask;
    internal nint pszText;
    internal int cchTextMax;
    internal int iImage;
    internal nint lParam;
}
