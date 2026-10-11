using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.Surfaces;

/// <summary>
/// Contador de taps encadenados extraído de <c>SystemTrayBroker.Fire/IsChainable</c>.
/// Una sola fuente de verdad de timing para el broker del tray y futuros adaptadores.
/// </summary>
internal sealed class TapChainCounter
{
    private Type? _lastTrigger;
    private int _lastCount;
    private int _lastTick;

    public int Next(Type trigger, int nowTick, uint doubleClickTimeMs)
    {
        int count = 1;
        if (IsChainable(trigger) && trigger == _lastTrigger)
        {
            if (unchecked(nowTick - _lastTick) <= (int)doubleClickTimeMs)
                count = Math.Min(_lastCount + 1, 10);
        }
        if (IsChainable(trigger))
        {
            _lastTrigger = trigger;
            _lastCount = count;
            _lastTick = nowTick;
        }
        else
        {
            _lastTrigger = null;
            _lastCount = 0;
        }
        return count;
    }

    public static bool IsChainable(Type trigger)
        => trigger == typeof(MainTap)
            || trigger == typeof(AlternativeTap1)
            || trigger == typeof(AlternativeTap2)
            || trigger == typeof(LongTap);
}
