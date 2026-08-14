using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IControl : IBaseWindow
{
    new string Name { get; set; }
    LayoutLength? LayoutWidth { get; set; }
    LayoutLength? LayoutHeight { get; set; }
    LayoutOptions LayoutOptions { get; set; }
    Thickness Margin { get; set; }
    Thickness Padding { get; set; }
    DockPosition Dock { get; set; }

    int GridRow { get; set; }
    int GridColumn { get; set; }
    int GridRowSpan { get; set; }
    int GridColumnSpan { get; set; }

    Color? BackgroundColor { get; set; }

    event Action? Clicked;
    void OnClick(Action handler);
}

internal interface IClickEventSource
{
    void RaiseClickInternal();
    void AddClickHandler(Action handler);
}

public delegate nint Win32MessageHandler(nint wParam, nint lParam);
