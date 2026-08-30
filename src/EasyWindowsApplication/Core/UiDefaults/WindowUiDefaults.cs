using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal sealed record WindowUiDefaults
{
    public int Width { get; init; } = 420;
    public int Height { get; init; } = 280;
    public WindowPositionOnScreen Position { get; init; } = WindowPositionOnScreen.Center;
    public Color? Background { get; init; } = null;
    public string Title { get; init; } = "";
    public string Name { get; init; } = "";
}
