using EasyWindowsApplication.CoreModule.Backend;

namespace EasyWindowsApplication.Share;

public delegate nint Win32MessageHandler(nint wParam, nint lParam);

public abstract class ControlBase<TSelf> : IControl, IClickEventSource where TSelf : ControlBase<TSelf>
{
    public nint Hwnd { get; internal set; }
    public string Name { get; internal set; } = "";

    private int _x, _y, _w, _h;

    // ── Typed events ──
    internal event Action? InternalClick;

    void IClickEventSource.RaiseClickInternal() => InternalClick?.Invoke();
    void IClickEventSource.AddClickHandler(Action handler) => InternalClick += handler;

    // ── Position / Size (raw setters) ──
    public int X
    {
        set { _x = value; ApplyBounds(); }
    }
    public int Y
    {
        set { _y = value; ApplyBounds(); }
    }
    public int W
    {
        set { _w = value; ApplyBounds(); }
    }
    public int H
    {
        set { _h = value; ApplyBounds(); }
    }

    public void SetBounds(int x, int y, int w, int h)
    {
        _x = x; _y = y; _w = w; _h = h;
        ApplyBounds();
    }

    internal void SetPositionDirect(int x, int y)
    {
        _x = x; _y = y;
    }

    internal void SetDimensionsDirect(int w, int h)
    {
        _w = w; _h = h;
    }

    internal void ApplyBounds()
    {
        if (Hwnd != 0)
            Win32.MoveWindow(Hwnd, _x, _y, _w, _h, true);
    }

    internal MasterRouter Router { get; set; } = default!;
    internal HandleRegistry Registry { get; set; } = default!;

    public TSelf OnMessage(uint msg, Win32MessageHandler handler)
    {
        Router.RegisterHandler(Hwnd, msg, handler);
        return (TSelf)this;
    }

    public TSelf OnClick(Action handler)
    {
        InternalClick += handler;
        return (TSelf)this;
    }
}
