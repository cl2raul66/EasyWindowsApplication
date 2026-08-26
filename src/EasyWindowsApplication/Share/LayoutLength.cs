
namespace EasyWindowsApplication.Share;

public readonly struct LayoutLength
{
    public float Value { get; }
    public GridUnitType Type { get; }

    private LayoutLength(float value, GridUnitType type)
    {
        Value = value;
        Type = type;
    }

    public static LayoutLength Auto => new(0, GridUnitType.Auto);
    public static LayoutLength Absolute(float pixels) => new(pixels, GridUnitType.Absolute);
    public static LayoutLength Star(float value = 1) => new(value, GridUnitType.Star);
}
