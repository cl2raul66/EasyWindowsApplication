using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Share;

public interface IMenuItem : IViewSurface, IText, IInputSurface
{
    bool IsEnabled { get; set; }
    bool IsChecked { get; set; }
}
