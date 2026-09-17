using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Menus;

internal sealed class MenuItem : IMenuItem
{
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

    public event Action? Clicked;

    public void OnClick(Action handler) => Clicked += handler;

    internal void RaiseClicked() => Clicked?.Invoke();
}
