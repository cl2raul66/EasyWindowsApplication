using System.ComponentModel;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.Share.Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ControlAccess
{
    private static IBehaviorServicesController? _controller;

    internal interface IBehaviorServicesController
    {
        T Get<T>(string name) where T : IControl;
        T GetWindow<T>(string name) where T : IBaseWindow;
    }

    internal static void SetController(IBehaviorServicesController? controller) => _controller = controller;

    public static T Get<T>(string name) where T : IControl
        => _controller!.Get<T>(name);

    public static T GetWindow<T>(string name) where T : IBaseWindow
        => _controller!.GetWindow<T>(name);
}