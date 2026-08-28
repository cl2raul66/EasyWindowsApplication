using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal sealed class Win32NativeHandleFactory : INativeHandleFactory
{
    public nint CreateHandle(nint parentHwnd, IControl control, HandleRegistry registry)
    {
        string windowClass;
        uint style;
        string text = "";

        // Resolver texto si implementa IText (IButton, ILabel)
        if (control is IText textControl)
            text = textControl.Text ?? "";

        // Determinación de clase y estilo base
        // Se usa WS_CHILD | WS_VISIBLE siempre; TABSTOP para botones
        if (control is IButton || control is Button)
        {
            windowClass = WC.BUTTON;
            style = WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.PUSHBUTTON;
        }
        else if (control is ILabel || control is Label)
        {
            windowClass = WC.STATIC;
            style = WS.CHILD | WS.VISIBLE | SS.LEFT;
        }
        else
        {
            // Fallback genérico: STATIC para controles no mapeados
            windowClass = WC.STATIC;
            style = WS.CHILD | WS.VISIBLE | SS.LEFT;
        }

        // Crear HWND via helper existente (aplica fuente por defecto)
        nint hwnd = ControlProcedures.CreateControl(
            windowClass,
            parentHwnd,
            style,
            0,
            text,
            0, 0, 0, 0,
            0);

        if (hwnd != 0)
        {
            // Asignar Hwnd al control (propiedad internal set en ControlBase)
            if (control is ControlBase cb)
                cb.Hwnd = hwnd;

            // Registrar en HandleRegistry (hwnd + name)
            registry.Register(hwnd, control);
            // Track parent->child para limpieza determinística en WM_DESTROY (Fase 5)
            if (parentHwnd != 0)
                registry.TrackChildWindow(parentHwnd, hwnd);
        }

        return hwnd;
    }
}
