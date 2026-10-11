namespace EasyWindowsApplication.Share;

/// <summary>Modo de visibilidad en capturas de pantalla (PrtSc, Teams, etc.).</summary>
public enum ScreenCaptureMode
{
    /// <summary>Contenido normal, capturable.</summary>
    Allow,

    /// <summary>La ventana aparece negra en captura (WDA_EXCLUDEFROMCAPTURE, Win10 2004+).</summary>
    HideContent,

    /// <summary>La ventana no aparece en captura (WDA_EXCLUDEFROMCAPTURE con fallback a WDA_MONITOR).</summary>
    HideWindow,
}
