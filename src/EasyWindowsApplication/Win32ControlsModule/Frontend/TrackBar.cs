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
    }

    public int PageSize
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETPAGEsize, 0, (nint)value);
    }

    public int ThumbLength
    {
        set => ControlProcedures.SendMessage(Hwnd, TBM.SETTHUMBLENGTH, (nint)value, 0);
    }
}
