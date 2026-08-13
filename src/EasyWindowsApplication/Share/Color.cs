namespace EasyWindowsApplication.Share;

public readonly struct Color
{
    private readonly int _value; // 0xAABBGGRR

    private Color(int value) => _value = value;

    public static Color FromArgb(byte a, byte r, byte g, byte b) =>
        new((a << 24) | (b << 16) | (g << 8) | r);

    public static Color FromRgb(byte r, byte g, byte b) =>
        FromArgb(255, r, g, b);

    public byte A => (byte)(_value >> 24);
    public byte R => (byte)_value;
    public byte G => (byte)(_value >> 8);
    public byte B => (byte)(_value >> 16);

    public bool IsTransparent => A == 0;

    internal int ToCOLORREF() => _value & 0x00FFFFFF;
    internal int ToARGB() => _value;

    public static Color White => FromRgb(255, 255, 255);
    public static Color Black => FromRgb(0, 0, 0);
    public static Color LightBlue => FromRgb(173, 216, 230);
    public static Color LightYellow => FromRgb(255, 255, 224);
    public static Color LightGray => FromRgb(211, 211, 211);
    public static Color Gray => FromRgb(128, 128, 128);
    public static Color DarkGray => FromRgb(64, 64, 64);
    public static Color Red => FromRgb(255, 0, 0);
    public static Color Green => FromRgb(0, 128, 0);
    public static Color Blue => FromRgb(0, 0, 255);
    public static Color Yellow => FromRgb(255, 255, 0);
    public static Color Orange => FromRgb(255, 165, 0);
    public static Color Pink => FromRgb(255, 192, 203);
    public static Color Cyan => FromRgb(0, 255, 255);
    public static Color Magenta => FromRgb(255, 0, 255);
    public static Color Transparent => FromArgb(0, 0, 0, 0);
    public static Color WhiteSmoke => FromRgb(245, 245, 245);
    public static Color LightCoral => FromRgb(240, 128, 128);
    public static Color LightGreen => FromRgb(144, 238, 144);
    public static Color LightSkyBlue => FromRgb(135, 206, 250);
    public static Color LightSalmon => FromRgb(255, 160, 122);
    public static Color LightSteelBlue => FromRgb(176, 196, 222);
}
