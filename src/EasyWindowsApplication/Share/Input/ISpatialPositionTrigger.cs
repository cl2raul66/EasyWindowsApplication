namespace EasyWindowsApplication.Share.Input;

public interface ISpatialPositionTrigger { }

public readonly struct Hover : ISpatialPositionTrigger { }

public readonly struct MainTap : ISpatialPositionTrigger { }

public readonly struct MainDoubleTap : ISpatialPositionTrigger { }

public readonly struct AlternativeTap1 : ISpatialPositionTrigger { }

public readonly struct AlternativeTap2 : ISpatialPositionTrigger { }

public readonly struct LongTap : ISpatialPositionTrigger { }

public readonly struct Holding : ISpatialPositionTrigger { }
