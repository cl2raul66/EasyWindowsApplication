using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public abstract class View<TSelf> : ControlBase where TSelf : View<TSelf>
{
    public new TSelf OnMessage(uint msg, Win32MessageHandler handler)
    {
        base.OnMessage(msg, handler);
        return (TSelf)this;
    }

    public new TSelf OnClick(Action handler)
    {
        base.OnClick(handler);
        return (TSelf)this;
    }
}
