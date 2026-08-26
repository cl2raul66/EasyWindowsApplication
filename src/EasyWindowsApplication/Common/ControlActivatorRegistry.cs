namespace EasyWindowsApplication.Common;

internal sealed class ControlActivatorRegistry
{
    private readonly Dictionary<Type, Func<object>> _factories = new();

    internal void Register<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory()!;
    }

    internal T Create<T>() where T : class
    {
        if (_factories.TryGetValue(typeof(T), out var f))
            return (T)f();
        throw new InvalidOperationException($"No control implementation registered for '{typeof(T).FullName}'.");
    }

    internal object CreateFor(Type type)
    {
        if (_factories.TryGetValue(type, out var f))
            return f();
        throw new InvalidOperationException($"No control implementation registered for '{type.FullName}'.");
    }

    internal static ControlActivatorRegistry Shared { get; } = new();
}
