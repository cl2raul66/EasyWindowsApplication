using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal sealed record ControlUiDefaults
{
    public int? PreferredHeight { get; init; }
    public Thickness? Padding { get; init; }
    public Color? Background { get; init; }
}
