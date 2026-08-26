namespace EasyWindowsApplication.Common;

internal sealed partial class ControlActivatorRegistry
{
    private readonly Dictionary<Type, Func<object>> _factories = new();
    private static bool _inited;
    private static readonly object _initLock = new();

    internal void Register<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory()!;
    }

    internal T Create<T>() where T : class
    {
        if (_factories.TryGetValue(typeof(T), out var f))
            return (T)f();
        throw new InvalidOperationException($"No control implementation registered for '{typeof(T).FullName}'. Did you forget to add InternalsVisibleTo or reference the plugin assembly?");
    }

    internal object CreateFor(Type type)
    {
        if (_factories.TryGetValue(type, out var f))
            return f();
        throw new InvalidOperationException($"No control implementation registered for '{type.FullName}'.");
    }

    internal static ControlActivatorRegistry Shared { get; } = new();

    internal static void EnsureInitialized()
    {
        lock (_initLock)
        {
            if (_inited) return;
            RegisterGeneratedActivators();
            _inited = true;
        }
    }

    static partial void RegisterGeneratedActivators();
}
