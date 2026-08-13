using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IChildrenBuilder
{
    IChildrenBuilder View<T>(Action<ControlBuilder<T>> configure) where T : ControlBase<T>, new();
}
