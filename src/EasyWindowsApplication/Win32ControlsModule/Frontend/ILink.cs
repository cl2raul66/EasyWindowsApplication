using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface ILink : IControl, IInputSurface
{
    string Text { get; set; }
}
