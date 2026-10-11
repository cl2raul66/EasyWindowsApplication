using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class WindowConfigBuilder : IWindowConfig
{
    private readonly WindowModel _model;

    internal WindowConfigBuilder(WindowModel model) => _model = model;

    public IWindowConfig Name(string name)
    {
        _model.Name = name;
        return this;
    }

    public IWindowConfig Title(string title)
    {
        _model.Title = title;
        return this;
    }

    public IWindowConfig Dimensions(int width, int height)
    {
        _model.Width = width;
        _model.Height = height;
        return this;
    }

    public IWindowConfig MinSize(int width, int height)
    {
        _model.MinSize = (width, height);
        return this;
    }

    public IWindowConfig MaxSize(int width, int height)
    {
        _model.MaxSize = (width, height);
        return this;
    }

    public IWindowConfig Position(IWindowPosition position)
    {
        _model.WindowPosition = position;
        return this;
    }

    [Obsolete("Use Position(IWindowPosition) con PositionPreset(WindowPositionPreset.Center) para lo mismo. Será eliminado en la próxima major.")]
    public IWindowConfig Position(WindowPositionOnScreen position)
    {
        _model.Position = position;
        return this;
    }

    public IWindowConfig Background(Color color)
    {
        _model.Background = color;
        return this;
    }

    public IWindowConfig Resizable(bool resizable) { _model.Resizable = resizable; return this; }
    public IWindowConfig CaptionButtons(CaptionButtons buttons) { _model.CaptionButtons = buttons; return this; }
    public IWindowConfig TopMost(bool topMost) { _model.TopMost = topMost; return this; }
    public IWindowConfig Opacity(double opacity) { _model.Opacity = Math.Clamp(opacity, 0.0, 1.0); return this; }
    public IWindowConfig SystemBackdrop(WindowBackdrop backdrop) { _model.Backdrop = backdrop; return this; }
    public IWindowConfig ScreenCapture(ScreenCaptureMode mode) { _model.ScreenCapture = mode; return this; }
    public IWindowConfig WindowState(WindowState state) { _model.InitialWindowState = state; return this; }

    public IWindowConfig TitleBar(Action<ITitleBarConfig> configure)
    {
        var tb = new TitleBarConfigBuilder();
        configure(tb);
        _model.TitleBar = tb.Model;
        return this;
    }

    public IWindowConfig Scroll(Action<IWindowsScrollConfig> configure)
    {
        var cfg = new WindowsScroll();
        configure(cfg);
        _model.ScrollConfig = cfg;
        return this;
    }

    public IWindowConfig SystemTray(Action<ISystemTray> configure)
    {
        _model.SystemTrayConfigure = configure;
        return this;
    }

    public IWindowContentConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout
        => RegisterContent(configure);

    public IWindowContentConfig Content(Action<IContentBuilder> configure)
        => RegisterContent(configure);

    private IWindowContentConfig RegisterContent(Action<IContentBuilder> configure)
    {
        var content = new ContentBuilderImpl();
        configure(content);
        _model.Content = content.Model;
        return new WindowContentConfig();
    }
}

internal sealed class WindowContentConfig : IWindowContentConfig { }

internal sealed class TitleBarConfigBuilder : ITitleBarConfig
{
    internal TitleBarBuilder Model { get; } = new();
    public TitleBarMode Mode => Model.Mode;

    public ITitleBarConfig Icon(nint hIcon) { Model.Icon = hIcon; return this; }
    public ITitleBarConfig Title(string title) { Model.Title = title; return this; }
    public ITitleBarConfig Custom<TView>(Action<TView> configure) where TView : class, IViewSurface
    {
        Model.Mode = TitleBarMode.Custom;
        Model.ConfigureCustom = v => configure((TView)v);
        return this;
    }
}
