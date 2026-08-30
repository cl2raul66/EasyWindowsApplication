using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal abstract class CoreUiDefaults : IDefaultUiValues
{
    public virtual WindowUiDefaults MainWindow => new()
    {
        Width = 420,
        Height = 280,
        Position = WindowPositionOnScreen.Center,
        Background = null
    };

    public virtual WindowUiDefaults AlternativeWindow => new()
    {
        Width = 400,
        Height = 300,
        Position = WindowPositionOnScreen.Center,
        Background = null
    };

    public abstract FontSpec DefaultFont { get; }

    public virtual Color DefaultForeground
        => ColorFromSysColor(Win32.GetSysColor(COLOR_WINDOWTEXT));

    public virtual Color DefaultBackground
        => ColorFromSysColor(Win32.GetSysColor(COLOR_WINDOW));

    public virtual Thickness DefaultPadding => new(3);
    public virtual int DefaultControlSpacing => 6;
    public virtual bool EnableVisualStyles => true;

    public virtual IReadOnlyDictionary<Type, ControlUiDefaults> ControlDefaults
        => _empty;

    private static readonly IReadOnlyDictionary<Type, ControlUiDefaults> _empty
        = new Dictionary<Type, ControlUiDefaults>();

    protected static Color ColorFromSysColor(int colorRef)
        => Color.FromRgb(
            (byte)(colorRef & 0xFF),
            (byte)((colorRef >> 8) & 0xFF),
            (byte)((colorRef >> 16) & 0xFF));

    // SysColor indices (winuser.h)
    private const int COLOR_WINDOW = 5;
    private const int COLOR_WINDOWTEXT = 8;
    private const int COLOR_BTNFACE = 15;
    private const int COLOR_BTNTEXT = 18;
}
