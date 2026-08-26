
namespace EasyWindowsApplication.Share;

public interface IWindow : IBaseWindow
{
    string Title { get; set; }
    float Width { get; set; }
    float Height { get; set; }
    WindowPositionOnScreen PositionMode { get; set; }
}
