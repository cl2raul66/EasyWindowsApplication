namespace EasyWindowsApplication.Core.Surfaces;

/// <summary>
/// Punto único de entrada OS → hub. Lo implementan las superficies con
/// alimentación externa (<see cref="ControlBase"/>, items de menú, ventanas).
/// </summary>
internal interface IInputFeed
{
    void FeedTrigger(Type trigger, int count);
}
