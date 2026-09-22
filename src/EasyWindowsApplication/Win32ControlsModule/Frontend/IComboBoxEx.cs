using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IComboBoxEx : IControl, IInputSurface
{
    string Text { get; set; }
    int SelectedIndex { get; set; }
    void AddItem(string text);
    void Clear();
    int Count { get; }
}
