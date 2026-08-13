namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IButton : IControl, IText
{
    bool Enabled { get; set; }
    void Click();
    void SetStyle(uint style, bool redraw = true);
}
