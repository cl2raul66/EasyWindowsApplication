namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IEdit : IControl
{
    string Text { get; set; }
    bool ReadOnly { get; set; }
    (int start, int end) Selection { get; set; }
    int LineCount { get; }
    bool Modified { get; set; }
    (int left, int right) Margins { get; set; }
    int TextLimit { get; set; }
    void SelectAll();
    void ClearSelection();
    void ReplaceSelection(string text);
    void ScrollCaret();
    bool CanUndo();
    void Undo();
    void EmptyUndoBuffer();
    int GetLine(int lineIndex);
    int GetLineLength(int charIndex);
    int GetFirstVisibleLine();
    string GetLineText(int lineIndex);
}
