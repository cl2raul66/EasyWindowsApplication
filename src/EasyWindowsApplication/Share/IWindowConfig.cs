namespace EasyWindowsApplication.Share;

public interface IWindowConfig
{
    IWindowConfig Name(string name);
    IWindowConfig Title(string title);
    IWindowConfig Dimensions(int width, int height);
    IWindowConfig MinSize(int width, int height);
    IWindowConfig MaxSize(int width, int height);

    /// <summary>
    /// Posición unificada (coordenada, preset o snap). Sustituye a <see cref="Position(WindowPositionOnScreen)"/>.
    /// </summary>
    IWindowConfig Position(IWindowPosition position);

    IWindowConfig Background(Color color);
    IWindowConfig Resizable(bool resizable);
    IWindowConfig CaptionButtons(CaptionButtons buttons);
    IWindowConfig TopMost(bool topMost);
    IWindowConfig Opacity(double opacity);
    IWindowConfig SystemBackdrop(WindowBackdrop backdrop);
    IWindowConfig ScreenCapture(ScreenCaptureMode mode);
    IWindowConfig WindowState(WindowState state);
    IWindowConfig TitleBar(Action<ITitleBarConfig> configure);
    IWindowConfig Scroll(Action<IWindowsScrollConfig> configure);
    IWindowConfig SystemTray(Action<ISystemTray> configure);
    IWindowContentConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout;
    IWindowContentConfig Content(Action<IContentBuilder> configure);

    [Obsolete("Use Position(IWindowPosition) con PositionPreset(WindowPositionPreset.Center) para lo mismo. Será eliminado en la próxima major.")]
    IWindowConfig Position(WindowPositionOnScreen position);
}

public interface IWindowContentConfig { }
