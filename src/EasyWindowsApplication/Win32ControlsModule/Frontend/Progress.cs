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

    public (int min, int max) Range32
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, PBM.SETRANGE32, (nint)value.min, (nint)value.max);
        }
    }

    public int Delta
    {
        set => ControlProcedures.SendMessage(Hwnd, PBM.DELTA, (nint)value, 0);
    }

    public int Step
    {
        set => ControlProcedures.SendMessage(Hwnd, PBM.SETSTEP, (nint)value, 0);
    }

    public void StepIt()
    {
        ControlProcedures.SendMessage(Hwnd, PBM.STEPIT, 0, 0);
    }

    public int BarColor
    {
        set => ControlProcedures.SendMessage(Hwnd, PBM.SETBARCOLOR, 0, (nint)value);
    }

    public int BkColor
    {
        set => ControlProcedures.SendMessage(Hwnd, PBM.SETBKCOLOR, 0, (nint)value);
    }

    public void SetMarquee(bool enable, int speed = 0)
    {
        ControlProcedures.SendMessage(Hwnd, PBM.SETMARQUEE, enable ? 1 : 0, (nint)speed);
    }

    public int GetStep()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, PBM.GETSTEP, 0, 0);
    }

    public int GetBarColor()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, PBM.GETBARCOLOR, 0, 0);
    }

    public int GetBkColor()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, PBM.GETBKCOLOR, 0, 0);
    }
}
