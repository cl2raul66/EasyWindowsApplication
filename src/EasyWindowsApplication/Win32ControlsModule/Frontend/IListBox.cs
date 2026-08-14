namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IListBox : IControl
{
    int SelectedIndex { get; set; }
    void AddItem(string text);
    void Clear();
    int Count { get; }
}
