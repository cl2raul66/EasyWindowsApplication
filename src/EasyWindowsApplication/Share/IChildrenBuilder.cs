using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Share;

public interface IChildrenBuilder
{
    IChildrenBuilder View<T>(Action<View<T>> configure) where T : class, IControl;
    IChildrenBuilder View<T>(Func<View<T>, View<T>> configure) where T : class, IControl;
    IChildrenBuilder View(Action<IViewBuilder> configure);

    IChildrenBuilder Row(int row);
    IChildrenBuilder Column(int column);
    IChildrenBuilder RowSpan(int span);
    IChildrenBuilder ColumnSpan(int span);

    IChildrenBuilder DockLeft();
    IChildrenBuilder DockTop();
    IChildrenBuilder DockRight();
    IChildrenBuilder DockBottom();
}
