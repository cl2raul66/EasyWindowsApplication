using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class ViewBuilderImpl : IViewBuilder
{
    internal ViewModel Model { get; } = new();

    public IViewBuilder Name(string name)
    {
        Model.Name = name;
        if (Model.Control is not null)
            Model.Control.Name = name;
        return this;
    }

    public IViewBuilder Margin(float uniform) => this;
    public IViewBuilder Margin(float vertical, float horizontal) => this;
    public IViewBuilder Margin(float top, float right, float bottom, float left) => this;
    public IViewBuilder HorizontalAlignment(LayoutAlignment alignment) => this;
    public IViewBuilder VerticalAlignment(LayoutAlignment alignment) => this;
    public IViewBuilder Width(float length) => this;
    public IViewBuilder Height(float length) => this;
    public IViewBuilder Padding(float uniform) => this;
    public IViewBuilder Padding(float vertical, float horizontal) => this;
    public IViewBuilder Padding(float top, float right, float bottom, float left) => this;
    public IViewBuilder Background(Color color) => this;

    public IViewBuilder Content(Action<IContentBuilder> configure)
    {
        var content = new ContentBuilderImpl();
        configure(content);
        Model.SubContent = content.Model;
        return this;
    }
}