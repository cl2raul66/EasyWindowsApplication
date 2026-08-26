using EasyWindowsApplication.Common;
using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class ChildrenBuilderImpl : IChildrenBuilder
{
    private readonly ContentModel _content;

    internal ChildrenBuilderImpl(ContentModel content) => _content = content;

    public IChildrenBuilder View<T>(Action<View<T>> configure) where T : class, IControl
    {
        var control = ControlActivatorRegistry.Shared.Create<T>();
        var view = new View<T>(control);
        configure(view);

        _content.Children.Add(new ViewModel
        {
            Name = view.Instance.Name,
            Control = view.Instance
        });
        return this;
    }

    public IChildrenBuilder View<T>(Func<View<T>, View<T>> configure) where T : class, IControl
    {
        var control = ControlActivatorRegistry.Shared.Create<T>();
        var view = new View<T>(control);
        var result = configure(view);

        _content.Children.Add(new ViewModel
        {
            Name = result.Instance.Name,
            Control = result.Instance
        });
        return this;
    }

    public IChildrenBuilder View(Action<IViewBuilder> configure)
    {
        var view = new ViewBuilderImpl();
        configure(view);
        _content.Children.Add(view.Model);
        return this;
    }

    public IChildrenBuilder Row(int row) => this;
    public IChildrenBuilder Column(int column) => this;
    public IChildrenBuilder RowSpan(int span) => this;
    public IChildrenBuilder ColumnSpan(int span) => this;
    public IChildrenBuilder DockLeft() => this;
    public IChildrenBuilder DockTop() => this;
    public IChildrenBuilder DockRight() => this;
    public IChildrenBuilder DockBottom() => this;
}
