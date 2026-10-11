using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IButton : IControl, IText, IInputSurface
{
    bool Enabled { get; set; }
    void SetStyle(uint style, bool redraw = true);
}
