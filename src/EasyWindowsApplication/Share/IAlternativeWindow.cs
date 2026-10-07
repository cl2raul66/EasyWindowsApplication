
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

    // Behavior-first lifecycle (with veto on Closing)
    void OnClosing(Action<CancelEventArgs> handler);
    void OnClosed(Action handler);

    // LEGACY
    event EventHandler<CancelEventArgs>? Closing;
    event EventHandler? Closed;
}
