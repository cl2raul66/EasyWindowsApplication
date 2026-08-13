namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IResourcesDictionary
{
    IResourcesDictionary Setting(Action<ISettingsBuilder> configure);
    IResourcesDictionary Services(Action<IServicesBuilder> configure);
}

public interface ISettingsBuilder
{
    ISettingsBuilder UseWinApi();
    ISettingsBuilder AppConfigFile(Action<IAppConfigFileBuilder> configure);
}

public interface IAppConfigFileBuilder
{
    IAppConfigFileBuilder Path(string path);
    IAppConfigFileBuilder WithAutoSave();
}

public interface IServicesBuilder
{
    IServicesBuilder Singleton<TService, TImplementation>();
}