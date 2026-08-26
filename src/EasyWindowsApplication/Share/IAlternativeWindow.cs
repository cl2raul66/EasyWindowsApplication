
namespace EasyWindowsApplication.Share;

public interface IAlternativeWindow : IBaseWindow
{
    nint OwnerHwnd { get; }
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    WindowPositionOnScreen PositionMode { get; set; }
}
