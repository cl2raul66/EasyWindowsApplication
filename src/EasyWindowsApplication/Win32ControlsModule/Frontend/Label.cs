using System.Runtime.InteropServices;
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

    public nint SetIcon(nint hIcon)
    {
        return ControlProcedures.SendMessage(Hwnd, STM.SETICON, hIcon, 0);
    }

    public nint GetIcon()
    {
        return ControlProcedures.SendMessage(Hwnd, STM.GETICON, 0, 0);
    }

    public nint SetImage(nint hImage, int imageType)
    {
        return ControlProcedures.SendMessage(Hwnd, STM.SETIMAGE, (nint)imageType, hImage);
    }

    public nint GetImage(int imageType)
    {
        return ControlProcedures.SendMessage(Hwnd, STM.GETIMAGE, (nint)imageType, 0);
    }
}
