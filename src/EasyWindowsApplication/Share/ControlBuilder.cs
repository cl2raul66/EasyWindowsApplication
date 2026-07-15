using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;

namespace EasyWindowsApplication.Share;

public sealed class ControlBuilder<T> where T : ControlBase<T>
{
    private readonly T _control;

    internal ControlBuilder(T control) => _control = control;

    public T Build() => _control;

    public ControlBuilder<T> Position(int x, int y)
    {
        _control.SetPositionDirect(x, y);
        _control.ApplyBounds();
        return this;
    }

    public ControlBuilder<T> Dimensions(int w, int h)
    {
        _control.SetDimensionsDirect(w, h);
        _control.ApplyBounds();
        return this;
    }

    public ControlBuilder<T> Name(string name)
    {
        _control.Name = name;
        return this;
    }

    public ControlBuilder<T> Text(string text)
    {
        nint ptr = Marshal.StringToHGlobalUni(text);
        Win32.SetWindowTextW(_control.Hwnd, ptr);
        Marshal.FreeHGlobal(ptr);
        return this;
    }
}
