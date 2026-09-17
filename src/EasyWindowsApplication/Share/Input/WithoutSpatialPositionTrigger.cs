namespace EasyWindowsApplication.Share.Input;

public interface WithoutSpatialPositionTrigger { }

public readonly struct KeyMenu : WithoutSpatialPositionTrigger { }

public readonly struct Chord<TMod, TKey> : WithoutSpatialPositionTrigger
    where TMod : IKeyMod
    where TKey : IKey
{
}
