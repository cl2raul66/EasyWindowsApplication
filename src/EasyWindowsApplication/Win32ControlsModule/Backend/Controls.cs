using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

public sealed class Button : ControlBase, IButton
{
    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? "";
            if (Hwnd != 0) ControlProcedures.SetWindowText(Hwnd, _text);
        }
    }
    public bool Enabled { get; set; } = true;
    public void Click() => OnClick(() => { });
    public void SetStyle(uint style, bool redraw = true) { }
}

public sealed class Label : ControlBase, ILabel
{
    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? "";
            if (Hwnd != 0) ControlProcedures.SetWindowText(Hwnd, _text);
        }
    }
}
