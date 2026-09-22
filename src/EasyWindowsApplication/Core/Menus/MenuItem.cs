using EasyWindowsApplication.Core.Surfaces;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.Menus;

internal sealed class MenuItem : IMenuItem, IInputFeed
{
    private readonly SurfaceInputHub _hub = new();
    void IInputFeed.FeedTrigger(Type trigger, int count) => _hub.Fire(trigger, count);

    public string Name { get; set; } = "";
    public string Text { get; set; } = "";
    public bool IsEnabled { get; set; } = true;
    public bool IsChecked { get; set; }
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

    public void OnInputWithoutSpatialPosition<TTrigger>(Action handler) where TTrigger : WithoutSpatialPositionTrigger
        => _hub.AddNonSpatial(typeof(TTrigger), handler);

    public void OnInputWithSpatialPosition<TTrigger>(Action handler) where TTrigger : ISpatialPositionTrigger
        => _hub.AddSpatial(typeof(TTrigger), handler);

    public void OnInputWithSpatialPosition<TTrigger, TCount>(Action handler)
        where TTrigger : ISpatialPositionTrigger
        where TCount : ITapCount
        => _hub.AddCounted(typeof(TTrigger), typeof(TCount), handler);
}
