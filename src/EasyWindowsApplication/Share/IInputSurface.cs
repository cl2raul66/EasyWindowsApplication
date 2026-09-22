using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Share;

/// <summary>
/// Contrato unificado de suscripción a entradas (opt-in por tipo).
/// Solo los tipos que lo declaran exponen <c>OnInput*</c> al IntelliSense.
/// </summary>
public interface IInputSurface
{
    void OnInputWithSpatialPosition<TTrigger>(Action handler)
        where TTrigger : ISpatialPositionTrigger;

    void OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCount;

    void OnInputWithoutSpatialPosition<TTrigger>(Action handler)
        where TTrigger : WithoutSpatialPositionTrigger;
}
