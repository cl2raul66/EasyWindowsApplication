using System.ComponentModel;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Share.Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class View<TSelf> : ControlBase where TSelf : View<TSelf>
{
    internal View() { }

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
