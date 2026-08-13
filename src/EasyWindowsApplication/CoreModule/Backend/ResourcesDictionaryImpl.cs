namespace EasyWindowsApplication.CoreModule.Backend;

using EasyWindowsApplication.CoreModule.Frontend;

internal sealed class ResourcesDictionaryImpl : IResourcesDictionary
{
    internal SettingsBuilderImpl SettingsContext { get; } = new();
    internal ServicesBuilderImpl ServicesContext { get; } = new();

    public IResourcesDictionary Setting(Action<ISettingsBuilder> configure)
    {
        configure(SettingsContext);
        return this;
    }

    public IResourcesDictionary Services(Action<IServicesBuilder> configure)
    {
        configure(ServicesContext);
        return this;
    }
}

internal sealed class SettingsBuilderImpl : ISettingsBuilder
{
    internal bool IsWinApiEnabled { get; private set; }
    internal AppConfigFileConfig AppConfig { get; } = new();

    public ISettingsBuilder UseWinApi()
    {
        IsWinApiEnabled = true;
        return this;
    }

    public ISettingsBuilder AppConfigFile(Action<IAppConfigFileBuilder> configure)
    {
        configure(new AppConfigFileBuilderImpl(AppConfig));
        return this;
    }
}

internal sealed class AppConfigFileConfig
{
    internal string Path { get; set; } = "";
    internal bool IsAutoSave { get; set; }
}

internal sealed class AppConfigFileBuilderImpl : IAppConfigFileBuilder
{
    private readonly AppConfigFileConfig _config;

    internal AppConfigFileBuilderImpl(AppConfigFileConfig config) => _config = config;

    public IAppConfigFileBuilder Path(string path)
    {
        _config.Path = path;
        return this;
    }

    public IAppConfigFileBuilder WithAutoSave()
    {
        _config.IsAutoSave = true;
        return this;
    }
}

internal sealed class ServicesBuilderImpl : IServicesBuilder
{
    private readonly Dictionary<Type, Type> _registrations = new();

    public IServicesBuilder Singleton<TService, TImplementation>()
    {
        _registrations[typeof(TService)] = typeof(TImplementation);
        return this;
    }

    internal IReadOnlyDictionary<Type, Type> Registrations => _registrations;
}