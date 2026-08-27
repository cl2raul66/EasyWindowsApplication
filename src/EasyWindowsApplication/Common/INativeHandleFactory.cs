using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Common;

internal interface INativeHandleFactory
{
    /// <summary>
    /// Crea el HWND nativo para el control y lo asigna a control.Hwnd.
    /// Debe registrar el control en HandleRegistry (hwnd + name).
    /// </summary>
    nint CreateHandle(nint parentHwnd, IControl control, HandleRegistry registry);
}
