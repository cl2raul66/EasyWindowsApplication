namespace EasyWindowsApplication.Core.Surfaces;

/// <summary>
/// Motor de dispatch unificado extraído de <c>SystemTrayImpl</c>.
/// Todo corre en el hilo de UI (message loop); sin locks.
/// </summary>
internal sealed class SurfaceInputHub
{
    private static readonly Type[] TapCounts =
    [
        typeof(Share.Input.OneTap), typeof(Share.Input.TwoTap), typeof(Share.Input.ThreeTap),
        typeof(Share.Input.FourTap), typeof(Share.Input.FiveTap), typeof(Share.Input.SixTap),
        typeof(Share.Input.SevenTap), typeof(Share.Input.EightTap), typeof(Share.Input.NineTap),
        typeof(Share.Input.TenTap)
    ];

    private readonly Dictionary<Type, List<Action>> _spatial = [];
    private readonly Dictionary<(Type Trigger, Type Count), List<Action>> _counted = [];
    private readonly Dictionary<Type, List<Action>> _nonSpatial = [];

    public void AddSpatial(Type trigger, Action handler)
    {
        if (!_spatial.TryGetValue(trigger, out var list))
        {
            list = [];
            _spatial[trigger] = list;
        }
        list.Add(handler);
    }

    public void AddCounted(Type trigger, Type count, Action handler)
    {
        var key = (trigger, count);
        if (!_counted.TryGetValue(key, out var list))
        {
            list = [];
            _counted[key] = list;
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
        if (_spatial.TryGetValue(trigger, out var list))
            foreach (var handler in list.ToArray()) handler();
        if (count >= 1 && count <= TapCounts.Length)
        {
            var key = (trigger, TapCounts[count - 1]);
            if (_counted.TryGetValue(key, out var counted))
                foreach (var handler in counted.ToArray()) handler();
        }
        if (_nonSpatial.TryGetValue(trigger, out var nonSpatial))
            foreach (var handler in nonSpatial.ToArray()) handler();
    }
}
