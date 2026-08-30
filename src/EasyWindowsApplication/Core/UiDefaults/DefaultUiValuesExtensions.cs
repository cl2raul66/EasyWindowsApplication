namespace EasyWindowsApplication.Core;

internal static class DefaultUiValuesExtensions
{
    public static ControlUiDefaults? GetFor(this IDefaultUiValues d, Type t)
        => d.ControlDefaults.TryGetValue(t, out var v) ? v : null;

    public static ControlUiDefaults? GetFor<T>(this IDefaultUiValues d)
        => d.GetFor(typeof(T));
}
