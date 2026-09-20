using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.SystemTray;

internal sealed class SystemTrayImpl : ISystemTray
{
    private static readonly Type[] TapCounts =
    {
        typeof(OneTap), typeof(TwoTap), typeof(ThreeTap), typeof(FourTap), typeof(FiveTap),
        typeof(SixTap), typeof(SevenTap), typeof(EightTap), typeof(NineTap), typeof(TenTap)
    };

    private readonly SystemTrayBroker _broker;
    private readonly Dictionary<Type, List<Action>> _spatial = new();
    private readonly Dictionary<(Type Trigger, Type Count), List<Action>> _counted = new();
    private readonly Dictionary<Type, List<Action>> _nonSpatial = new();
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

    public event Action? Clicked;

    public void OnClick(Action handler) => Clicked += handler;

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

    public ISystemTray OnInputWithSpatialPosition<TTrigger>(Action handler)
        where TTrigger : ISpatialPositionTrigger
    {
        Add(_spatial, typeof(TTrigger), handler);
        return this;
    }

    public ISystemTray OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCount
    {
        var key = (typeof(TTrigger), typeof(TCount));
        if (!_counted.TryGetValue(key, out var list))
        {
            list = new List<Action>();
            _counted[key] = list;
        }
        list.Add(handler);
        return this;
    }

    public ISystemTray OnInputWithoutSpatialPosition<TTrigger>(Action handler)
        where TTrigger : WithoutSpatialPositionTrigger
    {
        Add(_nonSpatial, typeof(TTrigger), handler);
        return this;
    }

    internal void DispatchTrigger(Type trigger, int count)
    {
        if (trigger == typeof(MainTap)) Clicked?.Invoke();
        if (_spatial.TryGetValue(trigger, out var list))
            foreach (var handler in list.ToArray()) handler();
        if (count >= 1 && count <= TapCounts.Length)
        {
            var key = (trigger, TapCounts[count - 1]);
            if (_counted.TryGetValue(key, out var counted))
                foreach (var handler in counted.ToArray()) handler();
        }
        if (_nonSpatial.TryGetValue(trigger, out var nonSpatial))
            foreach (var handler in nonSpatial.ToArray()) handler();
    }

    private static void Add(Dictionary<Type, List<Action>> map, Type trigger, Action handler)
    {
        if (!map.TryGetValue(trigger, out var list))
        {
            list = new List<Action>();
            map[trigger] = list;
        }
        list.Add(handler);
    }
}
