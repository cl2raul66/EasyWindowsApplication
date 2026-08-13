using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Frontend;

public interface IDockLayout : ILayout
{
    DockPosition DefaultDock { get; }
    bool ShouldExpandLastChild { get; }
}
