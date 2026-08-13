using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Frontend;

public interface IDockable
{
    DockPosition Dock { get; }
}
