using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class IpAddress : ControlBase<IpAddress>
{
    public void SetAddress(byte b1, byte b2, byte b3, byte b4)
    {
        nint packed = (nint)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
        ControlProcedures.SendMessage(Hwnd, IPM.SETADDRESS, 0, packed);
    }

    public (byte b1, byte b2, byte b3, byte b4) GetAddress()
    {
        nint result = ControlProcedures.SendMessage(Hwnd, IPM.GETADDRESS, 0, 0);
        byte b1 = (byte)((result >> 24) & 0xFF);
        byte b2 = (byte)((result >> 16) & 0xFF);
        byte b3 = (byte)((result >> 8) & 0xFF);
        byte b4 = (byte)(result & 0xFF);
        return (b1, b2, b3, b4);
    }

    public void Clear()
    {
        ControlProcedures.SendMessage(Hwnd, IPM.CLEARADDRESS, 0, 0);
    }

    public void SetRange(int field, byte low, byte high)
    {
        nint packed = (nint)((high << 8) | low);
        ControlProcedures.SendMessage(Hwnd, IPM.SETRANGE, (nint)field, packed);
    }

    public void SetFocusField(int field)
    {
        ControlProcedures.SendMessage(Hwnd, IPM.SETFOCUS, (nint)field, 0);
    }

    public bool IsBlank
    {
        get => ControlProcedures.SendMessage(Hwnd, IPM.ISBLANK, 0, 0) != 0;
    }
}
