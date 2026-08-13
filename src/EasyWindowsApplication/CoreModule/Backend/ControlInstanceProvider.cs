using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Backend;

internal static class ControlInstanceProvider
{
    internal static T Create<T>() where T : IControl
        => (T)CreateFor(typeof(T));

    internal static IControl CreateFor(Type type)
    {
        if (type == typeof(IButton))
            return new Win32ControlsModule.Backend.Button();
        if (type == typeof(Win32ControlsModule.Backend.Button))
            return new Win32ControlsModule.Backend.Button();
        if (type == typeof(ILabel))
            return new Win32ControlsModule.Backend.Label();
        if (type == typeof(Win32ControlsModule.Backend.Label))
            return new Win32ControlsModule.Backend.Label();

        throw new InvalidOperationException($"No control implementation registered for '{type.FullName}'.");
    }
}