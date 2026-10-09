using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal interface IContentModel;
internal interface IViewModel;

internal sealed class WindowModel
{
    internal string Name { get; set; } = "";
    internal string Title { get; set; } = "";
    internal int Width { get; set; } = 420;
    internal int Height { get; set; } = 280;
    internal WindowPositionOnScreen Position { get; set; } = WindowPositionOnScreen.Center;
    internal Color? Background { get; set; }
    internal bool IsAlternative { get; set; }
    internal IContentModel? Content { get; set; }
    internal WindowsScroll? ScrollConfig { get; set; }
    internal Action<ISystemTray>? SystemTrayConfigure { get; set; }
    internal Type? SurfaceType { get; set; }
    internal Action<IViewSurface>? ConfigureSurface { get; set; }

    // Phase A/B — #47
    internal TitleBarBuilder? TitleBar { get; set; }
    internal (int W, int H)? MinSize { get; set; }
    internal (int W, int H)? MaxSize { get; set; }
    internal bool Resizable { get; set; } = true;
    internal CaptionButtons CaptionButtons { get; set; } = CaptionButtons.All;
    internal bool TopMost { get; set; }
    internal double Opacity { get; set; } = 1.0;
    internal WindowBackdrop Backdrop { get; set; } = WindowBackdrop.None;
    internal ScreenCaptureMode ScreenCapture { get; set; } = ScreenCaptureMode.Allow;
    internal WindowState InitialWindowState { get; set; } = WindowState.Normal;
    internal IWindowPosition? WindowPosition { get; set; }
}

/// <summary>Config capturada de <see cref="ITitleBarConfig"/> (icono/título/custom view).</summary>
internal sealed class TitleBarBuilder
{
    internal TitleBarMode Mode { get; set; } = TitleBarMode.Simple;
    internal nint Icon { get; set; }
    internal string? Title { get; set; }
    internal Action<IViewSurface>? ConfigureCustom { get; set; }
}

internal sealed class ContentModel : IContentModel
{
    internal float Spacing { get; set; }
    internal Thickness Padding { get; set; }
    internal List<IViewModel> Children { get; } = new();
}

internal sealed class ViewModel : IViewModel
{
    internal string Name { get; set; } = "";
    internal IViewSurface? Control { get; set; }
    internal ContentModel? SubContent { get; set; }
    internal Type? ControlType { get; set; }
    internal Action<IViewSurface>? Configure { get; set; }
}
