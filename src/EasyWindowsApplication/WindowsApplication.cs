using EasyWindowsApplication.CoreModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication;

public static class WindowsApplication
{
    public static IApplicationLayoutPhase Resources() { return new ApplicationContext(); }
    public static IApplicationLayoutPhase Resources(Action<IResourcesDictionary> configure)
    {
        var context = new ApplicationContext();
        configure(context.Resources);
        return context;
    }


    public static IApplicationBehaviorPhase Layout() { return new ApplicationContext(); }
    public static IApplicationBehaviorPhase Layout(Action<ILayoutBuilder> configure)
    {
        var context = new ApplicationContext();
        configure(context.LayoutBuilder);
        return context;
    }
}
