using System.Runtime.InteropServices;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Link : ControlBase<Link>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }
}
