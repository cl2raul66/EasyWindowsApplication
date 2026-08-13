using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IChildrenBuilder
{
    IChildrenBuilder View<T>(Action<T> configure) where T : IControl;
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
