namespace EasyWindowsApplication.Share;

/// <summary>Botones de caption nativos.</summary>
[Flags]
public enum CaptionButtons
{
    None = 0,
    Close = 1,
    Minimize = 2,
    Maximize = 4,
    All = Close | Minimize | Maximize,
}
