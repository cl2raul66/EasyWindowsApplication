using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class RadioButton : ControlBase<RadioButton>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }

    public bool Checked
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, 0x00F0 /* BM_GETCHECK */, 0, 0) == 1;
        set => ControlProcedures.SendMessage(Hwnd, 0x00F1 /* BM_SETCHECK */, value ? 1 : 0, 0);
    }
}
