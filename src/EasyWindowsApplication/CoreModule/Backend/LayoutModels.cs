using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.CoreModule.Backend;

internal interface IContentModel;
internal interface IViewModel;

internal sealed class WindowModel
{
    internal string Name { get; set; } = "";
    internal string Title { get; set; } = "";
    internal int Width { get; set; } = 420;
    internal int Height { get; set; } = 280;
    internal WindowPositionOnScreen Position { get; set; } = WindowPositionOnScreen.Center;
    internal Color? Background { get; set; }
    internal bool IsAlternative { get; set; }
    internal IContentModel? Content { get; set; }
}

internal sealed class ContentModel : IContentModel
{
    internal float Spacing { get; set; }
    internal Thickness Padding { get; set; }
    internal List<IViewModel> Children { get; } = new();
}

internal sealed class ViewModel : IViewModel
{
    internal string Name { get; set; } = "";
    internal IControl? Control { get; set; }
    internal ContentModel? SubContent { get; set; }
}