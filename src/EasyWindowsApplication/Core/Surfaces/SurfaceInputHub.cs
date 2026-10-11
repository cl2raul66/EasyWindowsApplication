namespace EasyWindowsApplication.Core.Surfaces;

/// <summary>
/// Motor de dispatch unificado extraído de <c>SystemTrayImpl</c>.
/// Clave unificada (Trigger, CountSpec) donde CountSpec ∈ {OneTap..TenTap, AnyTap, NoTap}.
/// Todo corre en el hilo de UI (message loop); sin locks.
/// </summary>
internal sealed class SurfaceInputHub
{
    private static readonly Type[] TapCountTypes =
    [
        typeof(Share.Input.OneTap), typeof(Share.Input.TwoTap), typeof(Share.Input.ThreeTap),
        typeof(Share.Input.FourTap), typeof(Share.Input.FiveTap), typeof(Share.Input.SixTap),
        typeof(Share.Input.SevenTap), typeof(Share.Input.EightTap), typeof(Share.Input.NineTap),
        typeof(Share.Input.TenTap)
    ];

    private readonly Dictionary<(Type Trigger, Type CountSpec), List<Action>> _spatial = [];
    private readonly Dictionary<Type, List<Action>> _nonSpatial = [];

    public void AddSpatial(Type trigger, Type countSpec, Action handler)
    {
        var key = (trigger, countSpec);
        if (!_spatial.TryGetValue(key, out var list))
        {
            list = [];
            _spatial[key] = list;
        }
        list.Add(handler);
    }

    public void AddNonSpatial(Type trigger, Action handler)
    {
        if (!_nonSpatial.TryGetValue(trigger, out var list))
        {
            list = [];
            _nonSpatial[trigger] = list;
        }
        list.Add(handler);
    }

    public void Fire(Type trigger, int count)
    {
        // 1. AnyTap handlers (cualquier count).
        if (_spatial.TryGetValue((trigger, typeof(Share.Input.AnyTap)), out var any))
            foreach (var handler in any.ToArray()) handler();

        // 2. Conteo específico: OneTap..TenTap (mapear int -> Type).
        if (count >= 1 && count <= TapCountTypes.Length)
        {
            var countType = TapCountTypes[count - 1];
            if (_spatial.TryGetValue((trigger, countType), out var specific))
                foreach (var handler in specific.ToArray()) handler();
        }

        // 3. NoTap handlers (Hover, Holding: triggers sin conteo).
        if (_spatial.TryGetValue((trigger, typeof(Share.Input.NoTap)), out var noTap))
            foreach (var handler in noTap.ToArray()) handler();

        // 4. Non-spatial (KeyMenu, Chord...).
        if (_nonSpatial.TryGetValue(trigger, out var nonSpatial))
            foreach (var handler in nonSpatial.ToArray()) handler();
    }
}
