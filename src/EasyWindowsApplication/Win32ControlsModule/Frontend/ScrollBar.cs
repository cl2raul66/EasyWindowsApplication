using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class ScrollBar : ControlBase<ScrollBar>
{
    public int Position
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, 0x0400 + 2 /* SBM_GETPOS */, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, 0x0400 + 7 /* SBM_SETPOS */, (nint)value, 1);
    }

    public (int min, int max) Range
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, 0x0400 + 6 /* SBM_SETRANGE */, (nint)value.min, (nint)value.max);
        }
    }

    public (int min, int max) Range32
    {
        set
        {
            ControlProcedures.SendMessage(Hwnd, 0x0400 + 11 /* SBM_SETRANGEREDRAW */, (nint)value.min, (nint)value.max);
        }
    }

    public int PageSize
    {
        set => ControlProcedures.SendMessage(Hwnd, 0x0400 + 8 /* SBM_SETPAGE */, (nint)value, 0);
    }
}
