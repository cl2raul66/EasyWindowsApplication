using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Share;

/// <summary>
/// Contrato unificado de suscripción a entradas (opt-in por tipo).
/// Solo los tipos que lo declaran exponen <c>OnInput*</c> al IntelliSense.
/// </summary>
public interface IInputSurface
{
    /// <summary>
    /// Suscribe un handler para <typeparamref name="TTrigger"/> con <see cref="AnyTap"/> (cualquier count).
    /// </summary>
    void OnInputWithSpatialPosition<TTrigger>(Action handler)
        where TTrigger : ISpatialPositionTrigger;

    /// <summary>
    /// Suscribe un handler para <typeparamref name="TTrigger"/> con la especificación <typeparamref name="TCount"/>.
    /// <typeparamref name="TCount"/> puede ser: <c>OneTap</c>..<c>TenTap</c>, <see cref="AnyTap"/> o <see cref="NoTap"/>.
    /// </summary>
    void OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCountSpec;

    /// <summary>
    /// Suscribe un handler para el trigger por defecto <see cref="MainTap"/> con conteo <see cref="OneTap"/>.
    /// </summary>
    void OnInputWithSpatialPosition(Action handler);

    void OnInputWithoutSpatialPosition<TTrigger>(Action handler)
        where TTrigger : WithoutSpatialPositionTrigger;
}
