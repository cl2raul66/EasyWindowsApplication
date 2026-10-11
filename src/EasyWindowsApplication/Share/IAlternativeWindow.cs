
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

    /// <summary>
    /// Registra un handler de "ventana cerrada". Se dispara en cada cierre del ciclo de vida
    /// (<c>Close()</c> con app viva) pero <b>no durante el teardown de la app</b>.
    /// Para código de apagado use <see cref="IAppBehavior.OnTerminated"/>.
    /// </summary>
    void OnClosed(Action handler);

    // LEGACY
    event EventHandler<CancelEventArgs>? Closing;
    event EventHandler? Closed;
}
