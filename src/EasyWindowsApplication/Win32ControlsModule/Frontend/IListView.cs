namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IListView : IControl
{
    void AddColumn(string text, int width);
    int AddItem(string text);
    void SetItem(int row, int col, string text);
    void ClearItems();
    int GetItemCount();
    string GetItemText(int index, int subItem = 0);
    void EnableFullRowSelect();
    int GetSelectedCount();
    void SetColumnWidth(int colIndex, int width);
    void DeleteItem(int index);
    void DeleteAllColumns();
    void EnsureVisible(int index);
    void SetItemImage(int index, int imageIndex);
}
