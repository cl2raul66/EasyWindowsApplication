using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Progress : ControlBase<Progress>
{
    public int Value
    {
        set => ControlProcedures.SendMessage(Hwnd, PBM.SETPOS, (nint)value, 0);
    }

    public (int min, int max) Range
    {
        set
        {
            nint packed = ((nint)value.max << 16) | (ushort)value.min;
            ControlProcedures.SendMessage(Hwnd, PBM.SETRANGE, 0, packed);
        }
    }

    public int CurrentValue
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, PBM.GETPOS, 0, 0);
    }
}
