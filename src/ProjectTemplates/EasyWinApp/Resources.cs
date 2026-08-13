using EasyWindowsApplication.CoreModule.Frontend;

namespace EasyWinApp;

public static class ResourcesConfig
{
    public static void ConfigureResources(IResourcesDictionary res) =>
        res.Setting(sb => sb
            .UseWinApi()
        );
}
