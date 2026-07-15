using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Edit : ControlBase<Edit>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }

    public bool ReadOnly
    {
        get => false;
        set
        {
            const int EM_SETREADONLY = 0x00CF;
            ControlProcedures.SendMessage(Hwnd, EM_SETREADONLY, value ? 1 : 0, 0);
        }
    }
}
