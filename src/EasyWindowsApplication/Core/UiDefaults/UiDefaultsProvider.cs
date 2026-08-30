namespace EasyWindowsApplication.Core;

internal static class UiDefaultsProvider
{
    private sealed class CoreFallbackDefaults : CoreUiDefaults
    {
        public override FontSpec DefaultFont => FontSpec.SystemTheme;
    }

    private static IDefaultUiValues _current = new CoreFallbackDefaults();

    public static IDefaultUiValues Current => _current;

    public static void Set(IDefaultUiValues defaults)
        => _current = defaults ?? throw new ArgumentNullException(nameof(defaults));
}
