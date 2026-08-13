using System.Runtime.InteropServices;
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
        get => (int)ControlProcedures.SendMessage(Hwnd, BM.GETCHECK, 0, 0) == 1;
        set => ControlProcedures.SendMessage(Hwnd, BM.SETCHECK, value ? 1 : 0, 0);
    }

    public void Click()
    {
        ControlProcedures.SendMessage(Hwnd, BM.CLICK, 0, 0);
    }

    public int GetState()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, BM.GETSTATE, 0, 0);
    }

    public void SetState(bool highlight)
    {
        ControlProcedures.SendMessage(Hwnd, BM.SETSTATE, highlight ? 1 : 0, 0);
    }

    public nint GetImage(int imageType = 0)
    {
        return ControlProcedures.SendMessage(Hwnd, BM.GETIMAGE, (nint)imageType, 0);
    }

    public void SetImage(nint hImage, int imageType = 0)
    {
        ControlProcedures.SendMessage(Hwnd, BM.SETIMAGE, (nint)imageType, hImage);
    }
}
