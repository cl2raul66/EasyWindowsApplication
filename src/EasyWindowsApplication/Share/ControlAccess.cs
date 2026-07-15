using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication;

public static class ControlAccess
{
    private static HandleRegistry? _registry;

    internal static void Initialize(HandleRegistry registry)
        => _registry = registry;

    public static T Get<T>(string name) where T : ControlBase<T>
    {
        if (_registry == null)
            throw new InvalidOperationException("Application not initialized.");

        return (T)_registry.GetByName(name)! ?? throw new InvalidOperationException($"Control '{name}' not found.");
    }
}
