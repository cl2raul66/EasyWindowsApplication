using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class TreeView : ControlBase<TreeView>
{
    public nint AddItem(string text, nint parentHandle = 0, nint insertAfter = 0)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new TVITEMW
            {
                mask = 0x0001, // TVIF_TEXT
                pszText = textPtr,
                cchTextMax = text.Length,
            };

            var insertStruct = new TVINSERTSTRUCTW
            {
                hParent = parentHandle,
                hInsertAfter = insertAfter,
                item = item,
            };

            nint structPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TVINSERTSTRUCTW>());
            try
            {
                Marshal.StructureToPtr(insertStruct, structPtr, false);
                return ControlProcedures.SendMessage(Hwnd, TVM.INSERTITEMW, 0, structPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void DeleteItem(nint hItem)
    {
        ControlProcedures.SendMessage(Hwnd, TVM.DELETEITEM, 0, hItem);
    }

    public void DeleteAllItems()
    {
        ControlProcedures.SendMessage(Hwnd, TVM.DELETEITEM, 0, -0x10001 /* TVI_ROOT */);
    }

    public string GetItemText(nint hItem)
    {
        var item = new TVITEMW
        {
            mask = 0x0001, // TVIF_TEXT
            hItem = hItem,
        };

        nint buffer = Marshal.AllocHGlobal(512 * 2);
        try
        {
            item.pszText = buffer;
            item.cchTextMax = 512;

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TVITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                ControlProcedures.SendMessage(Hwnd, TVM.GETITEMW, 0, itemPtr);
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

    public void SelectItem(nint hItem)
    {
        ControlProcedures.SendMessage(Hwnd, TVM.SELECTITEM, (nint)TVGN.CARET, hItem);
    }

    public nint GetSelection()
    {
        return ControlProcedures.SendMessage(Hwnd, TVM.GETNEXTITEM, (nint)TVGN.CARET, 0);
    }

    public nint GetRoot()
    {
        return ControlProcedures.SendMessage(Hwnd, TVM.GETNEXTITEM, (nint)TVGN.FIRSTVISIBLE, 0);
    }

    public nint GetChild(nint hItem)
    {
        return ControlProcedures.SendMessage(Hwnd, TVM.GETNEXTITEM, (nint)TVGN.CHILD, hItem);
    }

    public nint GetNextSibling(nint hItem)
    {
        return ControlProcedures.SendMessage(Hwnd, TVM.GETNEXTITEM, (nint)TVGN.NEXT, hItem);
    }

    public void Expand(nint hItem)
    {
        ControlProcedures.SendMessage(Hwnd, TVM.EXPAND, (nint)2 /* TVE_EXPAND */, hItem);
    }

    public void Collapse(nint hItem)
    {
        ControlProcedures.SendMessage(Hwnd, TVM.EXPAND, (nint)1 /* TVE_COLLAPSE */, hItem);
    }

    public void EnableFullRowSelect()
    {
        int exStyle = (int)ControlProcedures.SendMessage(Hwnd, 0x112D /* TVM_GETEXTENDEDSTYLE */, 0, 0);
        exStyle |= 0x0004; // TVS_EX_FULLROWSELECT
        ControlProcedures.SendMessage(Hwnd, 0x112C /* TVM_SETEXTENDEDSTYLE */, 0, (nint)exStyle);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct TVITEMW
{
    internal uint mask;
    internal nint hItem;
    internal uint state;
    internal uint stateMask;
    internal nint pszText;
    internal int cchTextMax;
    internal int iImage;
    internal int iSelectedImage;
    internal nint cChildren;
    internal nint lParam;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct TVINSERTSTRUCTW
{
    internal nint hParent;
    internal nint hInsertAfter;
    internal TVITEMW item;
}
