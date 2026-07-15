using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ComboBoxEx : ControlBase<ComboBoxEx>
{
    public int AddItem(string text, int imageIndex = -1)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new COMBOBOXEXITEMW
            {
                mask = 0x0001, // CBEIF_TEXT
                iItem = GetCount(),
                pszText = textPtr,
                cchTextMax = text.Length,
                iImage = imageIndex,
                iSelectedImage = imageIndex,
            };

            nint itemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<COMBOBOXEXITEMW>());
            try
            {
                Marshal.StructureToPtr(item, itemPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, CBES.CBEM.INSERTITEMW, 0, itemPtr);
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
        ControlProcedures.SendMessage(Hwnd, 0x1404 /* CBEM_DELETEITEM */, (nint)index, 0);
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

    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct COMBOBOXEXITEMW
{
    internal uint mask;
    internal int iItem;
    internal nint pszText;
    internal int cchTextMax;
    internal int iImage;
    internal int iSelectedImage;
    internal int iOverlay;
    internal int iIndent;
    internal nint lParam;
}
