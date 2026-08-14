using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.CoreModule.Frontend;
using EasyWindowsApplication.LayoutModule.Frontend;

namespace EasyWindowsApplication;

public static class WindowsApplication
{
    public static IApplicationLayoutPhase Resources() => new Application();

    public static IApplicationLayoutPhase Resources(Action<IResourcesDictionary> configure)
    {
        var app = new Application();
        app.Resources(configure);
        return app;
    }

    public static IApplicationPostLayoutPhase Layout() => new Application();

    public static IApplicationPostLayoutPhase Layout(Action<ILayoutBuilder> configure)
        => new Application().Layout(configure);
}