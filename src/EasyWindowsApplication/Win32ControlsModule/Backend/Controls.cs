using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

public sealed class Button : ControlBase, IButton
{
    public string Text { get; set; } = "";
    public bool Enabled { get; set; } = true;
    public void Click() => OnClick(() => { });
    public void SetStyle(uint style, bool redraw = true) { }
}

public sealed class Label : ControlBase, ILabel
{
    public string Text { get; set; } = "";
}
