namespace EasyWindowsApplication.Share.Input;

public interface IKey { }

public interface IKeyMod { }

public readonly struct KeyShift : IKeyMod { }

public readonly struct KeyCtrl : IKeyMod { }

public readonly struct KeyAlt : IKeyMod { }

public readonly struct KeyWin : IKeyMod { }

public readonly struct KeyF1 : IKey { }

public readonly struct KeyF2 : IKey { }

public readonly struct KeyF3 : IKey { }

public readonly struct KeyF4 : IKey { }

public readonly struct KeyF5 : IKey { }

public readonly struct KeyF6 : IKey { }

public readonly struct KeyF7 : IKey { }

public readonly struct KeyF8 : IKey { }

public readonly struct KeyF9 : IKey { }

public readonly struct KeyF10 : IKey { }

public readonly struct KeyF11 : IKey { }

public readonly struct KeyF12 : IKey { }

public readonly struct KeyEnter : IKey { }

public readonly struct KeySpace : IKey { }

public readonly struct KeyEscape : IKey { }
