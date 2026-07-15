using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Header : ControlBase<Header>
{
    public int AddItem(string text, int width)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new HDITEMW
            {
                mask = 0x0001 | 0x0002, // HDI_TEXT | HDI_WIDTH
                pszText = textPtr,
                cchTextMax = text.Length,
                cxy = width,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<HDITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, HDM.INSERTITEMW, (nint)GetItemCount(), itemPtr);
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

    public void DeleteItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, HDM.DELETEITEM, (nint)index, 0);
    }

    public int GetItemCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, HDM.GETITEMCOUNT, 0, 0);
    }

    public void SetItemWidth(int index, int width)
    {
        var item = new HDITEMW
        {
            mask = 0x0002, // HDI_WIDTH
            cxy = width,
        };

        nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<HDITEMW>());
        try
        {
            Marshal.StructureToPtr(item, itemPtr, false);
            ControlProcedures.SendMessage(Hwnd, HDM.SETITEMW, (nint)index, itemPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(itemPtr);
        }
    }

    public string GetItemText(int index)
    {
        nint buffer = Marshal.AllocHGlobal(256 * 2);
        try
        {
            var item = new HDITEMW
            {
                mask = 0x0001, // HDI_TEXT
                pszText = buffer,
                cchTextMax = 256,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<HDITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                ControlProcedures.SendMessage(Hwnd, HDM.GETITEMW, (nint)index, itemPtr);
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

    public int GetItemWidth(int index)
    {
        var item = new HDITEMW
        {
            mask = 0x0002, // HDI_WIDTH
        };

        nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<HDITEMW>());
        try
        {
            Marshal.StructureToPtr(item, itemPtr, false);
            ControlProcedures.SendMessage(Hwnd, HDM.GETITEMW, (nint)index, itemPtr);
            return Marshal.PtrToStructure<HDITEMW>(itemPtr).cxy;
        }
        finally
        {
            Marshal.FreeHGlobal(itemPtr);
        }
    }

    public int OrderToIndex(int order)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, 0x120F /* HDM_ORDERTOINDEX */, (nint)order, 0);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct HDITEMW
{
    internal uint mask;
    internal int cxy;
    internal nint pszText;
    internal int cchTextMax;
    internal int fmt;
    internal nint lParam;
    internal int iImage;
    internal int iOrder;
    internal uint type;
    internal nint pvFilter;
    internal uint state;
}
