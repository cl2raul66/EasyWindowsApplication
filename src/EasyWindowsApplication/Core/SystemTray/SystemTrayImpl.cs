using EasyWindowsApplication.Core.Surfaces;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.SystemTray;

internal sealed class SystemTrayImpl : ISystemTray
{
    private readonly SystemTrayBroker _broker;
    private readonly SurfaceInputHub _hub = new();
    private string _tooltip = "";

    internal SystemTrayBroker Broker => _broker;

    internal SystemTrayImpl(SystemTrayBroker broker) => _broker = broker;

    public string Name { get; set; } = "SystemTray";
    public LayoutLength? LayoutWidth { get; set; }
    public LayoutLength? LayoutHeight { get; set; }
    public LayoutOptions LayoutOptions { get; set; } = new();
    public Thickness Margin { get; set; }
    public Thickness Padding { get; set; }
    public DockPosition Dock { get; set; }
    public int GridRow { get; set; }
    public int GridColumn { get; set; }
    public int GridRowSpan { get; set; }
    public int GridColumnSpan { get; set; }
    public Color? BackgroundColor { get; set; }

    public void Visibility(bool visible) => _broker.SetVisible(visible);

    public ISystemTray Tooltip(string text)
    {
        _tooltip = text;
        _broker.UpdateTooltip(text);
        return this;
    }

    public ISystemTray Notify(string title, string message)
    {
        _broker.ShowBalloon(title, message);
        return this;
    }

    public ISystemTray DismissNotification()
    {
        _broker.HideBalloon();
        return this;
    }

    public void OnInputWithSpatialPosition<TTrigger>(Action handler)
        where TTrigger : ISpatialPositionTrigger
    {
        _hub.AddSpatial(typeof(TTrigger), handler);
    }

    public void OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCount
    {
        _hub.AddCounted(typeof(TTrigger), typeof(TCount), handler);
    }

    public void OnInputWithoutSpatialPosition<TTrigger>(Action handler)
        where TTrigger : WithoutSpatialPositionTrigger
    {
        _hub.AddNonSpatial(typeof(TTrigger), handler);
    }

    internal void DispatchTrigger(Type trigger, int count) => _hub.Fire(trigger, count);
}
