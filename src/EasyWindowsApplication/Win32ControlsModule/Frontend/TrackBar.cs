using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class TrackBar : ControlBase<TrackBar>
{
    public int Value
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETPOS, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETPOS, 1, (nint)value);
    }

    public (int min, int max) Range
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, TBM.SETRANGEMIN, 1, (nint)value.min);
            ControlProcedures.SendMessage(Hwnd, TBM.SETRANGEMAX, 1, (nint)value.max);
        }
    }

    public int TickFrequency
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETTICFREQ, (nint)value, 0);
    }

    public int LineSize
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETLINESIZE, 0, (nint)value);
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETLINESIZE, 0, 0);
    }

    public int PageSize
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETPAGEsize, 0, (nint)value);
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETPAGEsize, 0, 0);
    }

    public int ThumbLength
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETTHUMBLENGTH, (nint)value, 0);
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETTHUMBLENGTH, 0, 0);
    }

    public int RangeMin
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETRANGEMIN, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETRANGEMIN, 1, (nint)value);
    }

    public int RangeMax
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETRANGEMAX, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETRANGEMAX, 1, (nint)value);
    }

    public (int start, int end) Selection
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, TBM.SETSEL, 1, (nint)((value.start & 0xFFFF) | ((value.end & 0xFFFF) << 16)));
        }
    }

    public int SelStart
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, TBM.GETSELSTART, 0, 0);
    }

    public void SetBuddy(nint hwndBuddy, bool left = true)
    {
        ControlProcedures.SendMessage(Hwnd, TBM.SETBUDDY, left ? 1 : 0, hwndBuddy);
    }

    public nint GetBuddy(bool left = true)
    {
        return ControlProcedures.SendMessage(Hwnd, TBM.GETBUDDY, left ? 1 : 0, 0);
    }
}
