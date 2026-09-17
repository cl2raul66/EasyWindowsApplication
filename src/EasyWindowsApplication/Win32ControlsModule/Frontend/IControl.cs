using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IControl : IBaseWindow, IViewSurface
{
    new string Name { get; set; }
}

internal interface IClickEventSource
{
    void RaiseClickInternal();
    void AddClickHandler(Action handler);
}

public delegate nint Win32MessageHandler(nint wParam, nint lParam);
