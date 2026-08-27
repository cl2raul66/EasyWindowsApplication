using System.Diagnostics.CodeAnalysis;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Common;

internal sealed partial class ControlActivatorRegistry
{
    private readonly Dictionary<Type, Func<object>> _factories = new();
    private readonly Dictionary<Type, INativeHandleFactory> _handleFactories = new();
    private static bool _inited;
    private static readonly object _initLock = new();

    internal void Register<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory()!;
    }

    internal void RegisterFactory<T>(INativeHandleFactory factory) where T : class
    {
        _handleFactories[typeof(T)] = factory;
    }

    internal T Create<T>() where T : class
    {
        EnsureInitialized();
        if (_factories.TryGetValue(typeof(T), out var f))
            return (T)f();
        throw new InvalidOperationException($"No control implementation registered for '{typeof(T).FullName}'. Did you forget to add InternalsVisibleTo or reference the plugin assembly?");
    }

    internal object CreateFor(Type type)
    {
        EnsureInitialized();
        if (_factories.TryGetValue(type, out var f))
            return f();
        throw new InvalidOperationException($"No control implementation registered for '{type.FullName}'.");
    }

    internal INativeHandleFactory? TryGetFactory(Type type)
    {
        EnsureInitialized();
        return _handleFactories.TryGetValue(type, out var f) ? f : null;
    }

    internal INativeHandleFactory CreateFactory<T>() where T : class
    {
        EnsureInitialized();
        if (_handleFactories.TryGetValue(typeof(T), out var f))
            return f;
        throw new InvalidOperationException($"No handle factory registered for '{typeof(T).FullName}'. Did you forget to register Win32NativeHandleFactory?");
    }

    internal INativeHandleFactory CreateFactoryFor(Type type)
    {
        EnsureInitialized();
        if (_handleFactories.TryGetValue(type, out var f))
            return f;
        throw new InvalidOperationException($"No handle factory registered for '{type.FullName}'.");
    }

    internal bool TryGetFactoryForControl(IControl control, out INativeHandleFactory? factory)
    {
        EnsureInitialized();
        var t = control.GetType();
        if (_handleFactories.TryGetValue(t, out factory))
            return true;
#pragma warning disable IL2075 // trimming: GetInterfaces usado solo para lookup de factories registradas (AOT: factories registradas vía source generator)
        foreach (var iface in t.GetInterfaces())
        {
            if (_handleFactories.TryGetValue(iface, out factory))
                return true;
        }
#pragma warning restore IL2075
        // fallback por base types
        var bt = t.BaseType;
        while (bt != null && bt != typeof(object))
        {
            if (_handleFactories.TryGetValue(bt, out factory))
                return true;
            bt = bt.BaseType;
        }
        factory = null;
        return false;
    }

    internal static ControlActivatorRegistry Shared { get; } = new();

    internal static void EnsureInitialized()
    {
        if (_inited) return;
        lock (_initLock)
        {
            if (_inited) return;
            RegisterGeneratedActivators();
            _inited = true;
        }
    }

    static partial void RegisterGeneratedActivators();
}
