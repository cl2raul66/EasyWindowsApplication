using System.Runtime.InteropServices;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ListView : ControlBase<ListView>
{
    public void AddColumn(string text, int width)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var column = new LVCOLUMNW
            {
                mask = LVCF.TEXT | LVCF.WIDTH,
                cx = width,
                pszText = textPtr,
                cchTextMax = text.Length,
                fmt = 0,
                iSubItem = 0,
                iImage = 0,
                iOrder = 0
            };

            nint columnPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVCOLUMNW>());
            try
            {
                Marshal.StructureToPtr(column, columnPtr, false);
                ControlProcedures.SendMessage(Hwnd, LVM.INSERTCOLUMNW, 0, columnPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(columnPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public int AddItem(string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new LVITEMW
            {
                mask = LVIF.TEXT,
                iItem = GetItemCount(),
                iSubItem = 0,
                pszText = textPtr,
                cchTextMax = text.Length,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, LVM.INSERTITEMW, 0, itemPtr);
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

    public void SetItem(int row, int col, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new LVITEMW
            {
                mask = LVIF.TEXT,
                iItem = row,
                iSubItem = col,
                pszText = textPtr,
                cchTextMax = text.Length,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                ControlProcedures.SendMessage(Hwnd, LVM.SETITEMTEXTW, 0, itemPtr);
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

    public void ClearItems()
    {
        ControlProcedures.SendMessage(Hwnd, LVM.DELETEALLITEMS, 0, 0);
    }

    public int GetItemCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LVM.GETITEMCOUNT, 0, 0);
    }

    public void EnableFullRowSelect()
    {
        nint exStyle = (nint)ControlProcedures.SendMessage(Hwnd, LVM.GETEXTENDEDLISTVIEWSTYLE, 0, 0);
        exStyle |= (nint)(LVS_EX.FULLROWSELECT | LVS_EX.DOUBLEBUFFER);
        ControlProcedures.SendMessage(Hwnd, LVM.SETEXTENDEDLISTVIEWSTYLE, 0, exStyle);
    }

    public int GetSelectedCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, LVM.GETSELECTEDCOUNT, 0, 0);
    }

    public void SetColumnWidth(int colIndex, int width)
    {
        ControlProcedures.SendMessage(Hwnd, LVM.SETCOLUMNWIDTH, (nint)colIndex, (nint)width);
    }

    public void DeleteItem(int index)
    {
        ControlProcedures.SendMessage(Hwnd, LVM.DELETEITEM, (nint)index, 0);
    }

    public string GetItemText(int index, int subItem = 0)
    {
        nint buffer = Marshal.AllocHGlobal(512 * 2);
        try
        {
            var item = new LVITEMW
            {
                mask = LVIF.TEXT,
                iItem = index,
                iSubItem = subItem,
                pszText = buffer,
                cchTextMax = 512,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                ControlProcedures.SendMessage(Hwnd, LVM.GETITEMW, 0, itemPtr);
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

    public void SetItemImage(int index, int imageIndex)
    {
        var item = new LVITEMW
        {
            mask = LVIF.IMAGE,
            iItem = index,
            iImage = imageIndex,
        };

        nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVITEMW>());
        try
        {
            Marshal.StructureToPtr(item, itemPtr, false);
            ControlProcedures.SendMessage(Hwnd, LVM.SETITEMW, 0, itemPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(itemPtr);
        }
    }

    public void EnsureVisible(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x1013 /* LVM_ENSUREVISIBLE */, (nint)index, 0);
    }

    public void DeleteAllColumns()
    {
        int count = (int)ControlProcedures.SendMessage(Hwnd, 0x101D /* LVM_GETHEADER */, 0, 0);
        for (int i = count - 1; i >= 0; i--)
        {
            ControlProcedures.SendMessage(Hwnd, 0x101C /* LVM_DELETECOLUMN */, (nint)i, 0);
        }
    }
}
