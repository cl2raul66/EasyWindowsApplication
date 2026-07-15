using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class UpDown : ControlBase<UpDown>
{
    public int Position
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, UDM.GETPOS, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETPOS, 0, (nint)value);
    }

    public int Position32
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, UDM.GETPOS32, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETPOS32, 0, (nint)value);
    }

    public (int min, int max) Range
    {
        set
        {
            nint packed = ((nint)value.max << 16) | (ushort)value.min;
            ControlProcedures.SendMessage(Hwnd, UDM.SETRANGE, 0, packed);
        }
    }

    public nint Buddy
    {
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETBUDDY, value, 0);
    }

    public int Base
    {
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETBASE, (nint)value, 0);
    }
}
