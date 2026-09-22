using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface ICheckBox : IControl, IInputSurface
{
    string Text { get; set; }
    bool Checked { get; set; }
}
