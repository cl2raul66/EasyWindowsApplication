using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

// Impl común de ISystemDialog: fluent Name/Title, props inertes de
// IViewSurface (los diálogos no tienen HWND propio) + OwnerProvider.
//
// Nota: IViewSurface ya declara la propiedad Name, así que los métodos
// fluent Name(string) se implementan explícitamente para evitar CS0102.
internal abstract class DialogSurfaceBase : ISystemDialog
{
    internal Func<nint> OwnerProvider { get; set; } = static () => 0;

    internal string DialogTitle { get; set; } = "";

    public string Name { get; set; } = "";
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

    ISystemDialog ISystemDialog.Name(string name)
    {
        Name = name;
        return this;
    }

    public ISystemDialog Title(string title)
    {
        DialogTitle = title;
        return this;
    }

    public void Visibility(bool visible)
    {
    }
}
