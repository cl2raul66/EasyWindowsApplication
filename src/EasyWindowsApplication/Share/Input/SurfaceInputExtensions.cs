using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Share;

/// <summary>
/// Extensiones ergonómicas sobre <see cref="IInputSurface"/>.
/// </summary>
public static class SurfaceInputExtensions
{
    public static void OnMainTap(this IInputSurface s, Action h)
        => s.OnInputWithSpatialPosition<MainTap>(h);

    public static void OnMainTap<TCount>(this IInputSurface s, Action h)
        where TCount : ITapCount
        => s.OnInputWithSpatialPosition<MainTap, TCount>(h);

    public static void OnAlternativeTap1(this IInputSurface s, Action h)
        => s.OnInputWithSpatialPosition<AlternativeTap1, OneTap>(h);

    public static void OnAlternativeTap2(this IInputSurface s, Action h)
        => s.OnInputWithSpatialPosition<AlternativeTap2, OneTap>(h);

    public static void OnKeyMenu(this IInputSurface s, Action h)
        => s.OnInputWithoutSpatialPosition<KeyMenu>(h);

    public static void OnChord<TMod, TKey>(this IInputSurface s, Action h)
        where TMod : IKeyMod where TKey : IKey
        => s.OnInputWithoutSpatialPosition<Chord<TMod, TKey>>(h);
}
