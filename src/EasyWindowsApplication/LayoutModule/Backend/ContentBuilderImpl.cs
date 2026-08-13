using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class ContentBuilderImpl : IContentBuilder
{
    internal ContentModel Model { get; } = new();

    public IContentBuilder Spacing(float pixels)
    {
        Model.Spacing = pixels;
        return this;
    }

    public IContentBuilder Padding(float uniform)
    {
        Model.Padding = new Thickness(uniform);
        return this;
    }

    public IContentBuilder Padding(float vertical, float horizontal)
    {
        Model.Padding = new Thickness(vertical, horizontal);
        return this;
    }

    public IContentBuilder Padding(float top, float right, float bottom, float left)
    {
        Model.Padding = new Thickness(top, right, bottom, left);
        return this;
    }

    public IContentBuilder Margin(float uniform) => this;
    public IContentBuilder Margin(float vertical, float horizontal) => this;
    public IContentBuilder Margin(float top, float right, float bottom, float left) => this;

    public IContentBuilder Children(Action<IChildrenBuilder> configure)
    {
        var children = new ChildrenBuilderImpl(Model);
        configure(children);
        return this;
    }

    public IContentBuilder RowDefinition(GridUnitType unitType, float value = 1) => this;
    public IContentBuilder ColumnDefinition(GridUnitType unitType, float value = 1) => this;
    public IContentBuilder RowSpacing(float pixels) => this;
    public IContentBuilder ColumnSpacing(float pixels) => this;
    public IContentBuilder DefaultDock(DockPosition position) => this;
    public IContentBuilder ShouldExpandLastChild(bool expand) => this;
}