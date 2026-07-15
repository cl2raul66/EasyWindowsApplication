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
