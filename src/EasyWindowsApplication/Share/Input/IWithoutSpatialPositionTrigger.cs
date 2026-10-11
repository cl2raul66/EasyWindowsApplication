namespace EasyWindowsApplication.Share.Input;

public interface IWithoutSpatialPositionTrigger { }

public readonly struct KeyMenu : IWithoutSpatialPositionTrigger { }
public readonly struct KeyEsc : IWithoutSpatialPositionTrigger { }

public readonly struct Chord<TMod, TKey> : IWithoutSpatialPositionTrigger
    where TMod : IKeyMod
    where TKey : IKey
{
}
