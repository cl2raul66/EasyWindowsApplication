using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class ChildrenBuilderImpl : IChildrenBuilder
{
    private readonly ContentModel _content;

    internal ChildrenBuilderImpl(ContentModel content) => _content = content;

    public IChildrenBuilder View<T>(Action<T> configure) where T : IControl
    {
        var control = ControlInstanceProvider.Create<T>();
        configure(control);

        _content.Children.Add(new ViewModel
        {
            Name = control.Name,
            Control = control
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