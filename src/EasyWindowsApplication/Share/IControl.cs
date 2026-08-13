namespace EasyWindowsApplication.Share;

public interface IControl
{
    nint Hwnd { get; }
    string Name { get; }
}

internal interface IClickEventSource
{
    void RaiseClickInternal();
    void AddClickHandler(Action handler);
}
