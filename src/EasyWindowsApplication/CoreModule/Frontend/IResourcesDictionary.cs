namespace EasyWindowsApplication.CoreModule.Frontend;

public interface IResourcesDictionary
{
    IResourcesDictionary UseWinApi();
}

public interface ISettingsBuilder
{
    ISettingsBuilder UseWinApi();
}

internal sealed class ResourcesDictionaryImpl : IResourcesDictionary
{
    internal bool IsWinApiEnabled { get; private set; }

    public IResourcesDictionary UseWinApi()
    {
        IsWinApiEnabled = true;
        return this;
    }
}

internal sealed class SettingsBuilderImpl : ISettingsBuilder
{
    internal bool IsWinApiEnabled { get; private set; }

    public ISettingsBuilder UseWinApi()
    {
        IsWinApiEnabled = true;
        return this;
    }
}
