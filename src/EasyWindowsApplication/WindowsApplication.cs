using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication;

public static class WindowsApplication
{
    public static IApplicationLayoutPhase Resources(Action<IResourcesDictionary> configure)
    {
        var app = new Application();
        app.Resources(configure);
        return app;
    }

    public static IApplicationPostLayoutPhase Layout(Action<ILayoutBuilder> configure)
        => new Application().Layout(configure);
}
