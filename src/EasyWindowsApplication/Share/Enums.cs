namespace EasyWindowsApplication.Share;

public enum WindowPositionOnScreen { Center, Manual }

public enum ScrollBarVisibility { Default, Always, Never }

public enum ScrollOrientation { Vertical, Horizontal, Both, Neither }


public enum GridUnitType { Absolute, Auto, Star }

public enum LayoutAlignment { Start, Center, End, Fill }

public enum DockPosition { Left, Top, Right, Bottom }

/// <summary>Botones de caption nativos.</summary>
[Flags]
public enum CaptionButtons { None = 0, Close = 1, Minimize = 2, Maximize = 4, All = Close | Minimize | Maximize }

/// <summary>Estado de la ventana principal.</summary>
public enum WindowState { Normal, Minimized, Maximized, FullScreen }

public enum WindowPositionPreset { Center, TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight }

public enum SnapRegion { Left, Right, Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight }

/// <summary>Fondo translúcido del shell (DWM). Requiere Windows 10 1809+ para efectos; en sistemas anteriores se ignora.</summary>
public enum WindowBackdrop { None, Mica, Acrylic, DesktopAcrylic, SolidColor }

/// <summary>Modo de title bar de la ventana principal.</summary>
public enum TitleBarMode
{
    /// <summary>Title bar nativa Win32 (WNDCLASSEX icon + SetWindowText).</summary>
    Simple,

    /// <summary>Custom title bar extendida sobre el client area (DwmExtendFrameIntoClientArea).</summary>
    Custom
}

/// <summary>Modo de visibilidad en capturas de pantalla (PrtSc, Teams, etc.).</summary>
public enum ScreenCaptureMode
{
    /// <summary>Contenido normal, capturable.</summary>
    Allow,

    /// <summary>La ventana aparece negra en captura (WDA_EXCLUDEFROMCAPTURE, Win10 2004+).</summary>
    HideContent,

    /// <summary>La ventana no aparece en captura (WDA_EXCLUDEFROMCAPTURE con fallback a WDA_MONITOR).</summary>
    HideWindow
}
