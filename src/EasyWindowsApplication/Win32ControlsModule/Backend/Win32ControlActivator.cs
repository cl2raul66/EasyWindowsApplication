using EasyWindowsApplication.Common;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal sealed class Win32ControlActivator : IControlActivator
{
    public void RegisterActivators(ControlActivatorRegistry registry)
    {
        registry.Register<IButton>(() => new Button());
        registry.Register<ILabel>(() => new Label());
        // Register concrete types as well for direct resolution if needed
        registry.Register<Button>(() => new Button());
        registry.Register<Label>(() => new Label());

        var factory = new Win32NativeHandleFactory();
        registry.RegisterFactory<IButton>(factory);
        registry.RegisterFactory<ILabel>(factory);
        registry.RegisterFactory<Button>(factory);
        registry.RegisterFactory<Label>(factory);
    }
}
