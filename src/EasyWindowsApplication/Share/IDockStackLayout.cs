
namespace EasyWindowsApplication.Share;

public interface IDockLayout : ILayout
{
    DockPosition DefaultDock { get; }
    bool ShouldExpandLastChild { get; }
}
