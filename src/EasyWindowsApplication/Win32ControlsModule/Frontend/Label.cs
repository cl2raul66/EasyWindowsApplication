using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Label : ControlBase<Label>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }
}
