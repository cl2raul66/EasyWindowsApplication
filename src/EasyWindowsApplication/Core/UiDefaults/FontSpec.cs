namespace EasyWindowsApplication.Core;

internal enum FontWeight { Normal, Bold }
internal enum FontStyle { Regular, Italic }

internal sealed record FontSpec
{
    public static readonly FontSpec SystemTheme = new() { IsSystemTheme = true };

    public bool IsSystemTheme { get; init; }
    public string Family { get; init; } = "Segoe UI";
    public float Size { get; init; } = 9f;
    public FontWeight Weight { get; init; } = FontWeight.Normal;
    public FontStyle Style { get; init; } = FontStyle.Regular;
}
