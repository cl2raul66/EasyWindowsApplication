using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed partial class Button : ControlBase<Button>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }

    public bool Enabled
    {
        get => (Win32.GetWindowLongPtrW(Hwnd, GWL.ID) & 0x10000000) != 0;
        set
        {
            int style = (int)Win32.GetWindowLongPtrW(Hwnd, GWL.STYLE);
            if (value)
                style |= (int)WS.VISIBLE;
            else
                style &= ~(int)WS.VISIBLE;
            Win32.SetWindowLongPtrW(Hwnd, GWL.STYLE, (nint)style);
        }
    }

    public void Click()
    {
        ControlProcedures.SendMessage(Hwnd, BM.CLICK, 0, 0);
    }

    public void SetImage(nint imageHandle, int imageType = 0)
    {
        ControlProcedures.SendMessage(Hwnd, BM.SETIMAGE, (nint)imageType, imageHandle);
    }

    public nint GetImage(int imageType = 0)
    {
        return ControlProcedures.SendMessage(Hwnd, BM.GETIMAGE, (nint)imageType, 0);
    }

    public void SetStyle(uint style, bool redraw = true)
    {
        ControlProcedures.SendMessage(Hwnd, BM.SETSTYLE, (nint)style, redraw ? 1 : 0);
    }

}
