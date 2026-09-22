namespace EasyWindowsApplication.Share;

public interface IViewSurface
{
    string Name { get; set; }
    LayoutLength? LayoutWidth { get; set; }
    LayoutLength? LayoutHeight { get; set; }
    LayoutOptions LayoutOptions { get; set; }
    Thickness Margin { get; set; }
    Thickness Padding { get; set; }
    DockPosition Dock { get; set; }

    int GridRow { get; set; }
    int GridColumn { get; set; }
    int GridRowSpan { get; set; }
    int GridColumnSpan { get; set; }

    Color? BackgroundColor { get; set; }

    void Visibility(bool visible)
    {
        if (this is IBaseWindow window)
        {
            if (visible) window.Show();
            else window.Hide();
        }
    }
}
