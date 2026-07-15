using EasyWindowsApplication.CoreModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication;

public static class WindowsApplication
{
    public static IApplicationLayoutPhase Resources() => new ApplicationContext();

    public static IApplicationLayoutPhase Resources(Action<IResourcesDictionary> configure)
    {
        var context = new ApplicationContext();
        configure(context.ResourcesDict);
        return context;
    }

    public static IApplicationBehaviorPhase Layout() => new ApplicationContext();

    public static IApplicationBehaviorPhase Layout(Action<ILayoutBuilder> configure)
    {
        var context = new ApplicationContext();
        return context.Layout(configure);
    }
}
