
namespace EasyWindowsApplication.Share;

public interface IAlternativeWindow : IBaseWindow, IInputSurface
{
    nint OwnerHwnd { get; }
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    WindowPositionOnScreen PositionMode { get; set; }
    void Visibility(bool visible);

    /// <summary>
    /// Destruye el HWND. Un Show() posterior re-crea a demanda.
    /// Pasa por el veto local Closing (no termina la app).
    /// </summary>
    void Close();

    event EventHandler<CancelEventArgs>? Closing;
    event EventHandler? Closed;
}
