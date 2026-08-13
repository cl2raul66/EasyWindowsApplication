using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;
using RECT = EasyWindowsApplication.CoreModule.Backend.RECT;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ToolTip : ControlBase<ToolTip>
{
    public void AddTool(nint hwnd, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var toolInfo = new TTTOOLINFOW
            {
                cbSize = (uint)Marshal.SizeOf<TTTOOLINFOW>(),
                hwnd = hwnd,
                uId = hwnd,
                uFlags = 0x0001, // TTF_SUBCLASS
                pszText = textPtr,
            };

            nint structPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TTTOOLINFOW>());
            try
            {
                Marshal.StructureToPtr(toolInfo, structPtr, false);
                ControlProcedures.SendMessage(Hwnd, TTM.ADDTOOL, 0, structPtr);
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

    public void DeleteTool(nint hwnd)
    {
        var toolInfo = new TTTOOLINFOW
        {
            cbSize = (uint)Marshal.SizeOf<TTTOOLINFOW>(),
            hwnd = hwnd,
            uId = hwnd,
        };

        nint structPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TTTOOLINFOW>());
        try
        {
            Marshal.StructureToPtr(toolInfo, structPtr, false);
            ControlProcedures.SendMessage(Hwnd, TTM.DELTOOL, 0, structPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(structPtr);
        }
    }

    public void UpdateText(nint hwnd, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            var toolInfo = new TTTOOLINFOW
            {
                cbSize = (uint)Marshal.SizeOf<TTTOOLINFOW>(),
                hwnd = hwnd,
                uId = hwnd,
                pszText = textPtr,
            };

            nint structPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TTTOOLINFOW>());
            try
            {
                Marshal.StructureToPtr(toolInfo, structPtr, false);
                ControlProcedures.SendMessage(Hwnd, TTM.UPDATETIPTEXT, 0, structPtr);
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

    public void Activate(bool activate)
    {
        ControlProcedures.SendMessage(Hwnd, activate ? TTM.ACTIVE : TTM.DEACTIVATE, 0, 0);
    }

    public void Pop()
    {
        ControlProcedures.SendMessage(Hwnd, TTM.POP, 0, 0);
    }

    public void SetDelayTime(int delay)
    {
        ControlProcedures.SendMessage(Hwnd, 0x0403 /* TTM_SETDELAYTIME */, 0, (nint)delay);
    }

    public int GetToolCount()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, TTM.GETTOOLCOUNT, 0, 0);
    }

    public void Update()
    {
        ControlProcedures.SendMessage(Hwnd, 0x0415 /* TTM_UPDATE */, 0, 0);
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct TTTOOLINFOW
{
    internal uint cbSize;
    internal uint uFlags;
    internal nint hwnd;
    internal nint uId;
    internal RECT rect;
    internal nint hinst;
    internal nint pszText;
    internal nint lParam;
    internal nint lpReserved;
}
