namespace EasyWindowsApplication.Share;

public interface IAppBehavior
{
    IAppBehavior OnLaunched(Action handler);
    IAppBehavior OnLaunched(Action<IAppBehavior> handler);
    IAppBehavior OnTerminating(Action<CancelEventArgs> handler);
    IAppBehavior OnTerminated(Action handler);

    IAppBehavior TaskbarButtonVisibility(bool visible);

    /// <summary>
    /// Única puerta de salida programática. Envía WM_CLOSE a la ventana
    /// principal (misma tubería que la X): pasa por el portón
    /// OnTerminating (vetable) y termina en OnTerminated.
    /// </summary>
    void Terminate();
}