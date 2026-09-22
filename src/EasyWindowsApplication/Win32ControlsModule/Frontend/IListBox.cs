using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IListBox : IControl, IInputSurface
{
    int SelectedIndex { get; set; }
    void AddItem(string text);
    void Clear();
    int Count { get; }
}
