namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IComboBoxEx : IControl
{
    string Text { get; set; }
    int SelectedIndex { get; set; }
    void AddItem(string text);
    void Clear();
    int Count { get; }
}
