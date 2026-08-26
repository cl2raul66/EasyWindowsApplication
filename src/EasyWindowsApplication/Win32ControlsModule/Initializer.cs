using System.Runtime.CompilerServices;
using EasyWindowsApplication.Common;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule;

internal static class Initializer
{
    [ModuleInitializer]
    internal static void Register()
    {
        var registry = ControlActivatorRegistry.Shared;
        new Win32ControlActivator().RegisterActivators(registry);
    }
}
