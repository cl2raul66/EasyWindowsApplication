namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IComboBox : IControl
{
    string Text { get; set; }
    int SelectedIndex { get; set; }
    void AddItem(string text);
    void Clear();
    int Count { get; }
}
