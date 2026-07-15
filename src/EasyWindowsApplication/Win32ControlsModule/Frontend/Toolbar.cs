using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Toolbar : ControlBase<Toolbar>
{
    public void SetButtonSize(int width, int height)
    {
        ControlProcedures.SendMessage(Hwnd, 0x041F /* TB_SETBUTTONSIZE */, (nint)width, (nint)height);
    }

    public int AddButton(int idCommand, string text, int imageIndex = -1)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var button = new TBBUTTONW
            {
                iBitmap = imageIndex,
                idCommand = idCommand,
                fsState = 0x0004, // TBSTATE_ENABLED
                fsStyle = 0x0000, // TBSTYLE_BUTTON
                dwData = 0,
                iString = textPtr,
            };

            nint btnPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TBBUTTONW>());
            try
            {
                Marshal.StructureToPtr(button, btnPtr, false);
                return (int)ControlProcedures.SendMessage(Hwnd, 0x0400 + 20 /* TB_ADDBUTTONSW */, 0, btnPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(btnPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void AddSeparator()
    {
        var button = new TBBUTTONW
        {
            iBitmap = 8,
            idCommand = 0,
            fsState = 0x0004, // TBSTATE_ENABLED
            fsStyle = 0x0001, // TBSTYLE_SEP
        };

        nint btnPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TBBUTTONW>());
        try
        {
            Marshal.StructureToPtr(button, btnPtr, false);
            ControlProcedures.SendMessage(Hwnd, 0x0400 + 20 /* TB_ADDBUTTONSW */, 0, btnPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(btnPtr);
        }
    }

    public void DeleteButton(int index)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0400 + 22 /* TB_DELETEBUTTON */, (nint)index, 0);
    }

    public int ButtonCount
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, 0x0400 + 24 /* TB_BUTTONCOUNT */, 0, 0);
    }

    public void EnableButton(int idCommand, bool enable)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0400 + 3 /* TB_ENABLEBUTTON */, (nint)idCommand, enable ? 1 : 0);
    }

    public void CheckButton(int idCommand, bool check)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0400 + 2 /* TB_CHECKBUTTON */, (nint)idCommand, check ? 1 : 0);
    }

    public void SetButtonText(int idCommand, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var button = new TBBUTTONW
            {
                idCommand = idCommand,
                fsState = 0x0004,
                fsStyle = 0x0000,
                iString = textPtr,
            };

            nint btnPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TBBUTTONW>());
            try
            {
                Marshal.StructureToPtr(button, btnPtr, false);
                ControlProcedures.SendMessage(Hwnd, 0x0400 + 63 /* TB_SETBUTTONINFOW */, (nint)idCommand, btnPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(btnPtr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct TBBUTTONW
{
    internal int iBitmap;
    internal int idCommand;
    internal byte fsState;
    internal byte fsStyle;
    internal byte bReserved0;
    internal byte bReserved1;
    internal nint dwData;
    internal nint iString;
}
