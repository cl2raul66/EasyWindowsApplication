using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class Edit : ControlBase<Edit>
{
    public string Text
    {
        get => ControlProcedures.GetWindowText(Hwnd);
        set => ControlProcedures.SetWindowText(Hwnd, value);
    }

    public bool ReadOnly
    {
        get
        {
            int style = (int)Win32.GetWindowLongPtrW(Hwnd, GWL.STYLE);
            return (style & 0x00000800) != 0; // ES_READONLY
        }
        set
        {
            ControlProcedures.SendMessage(Hwnd, EM.SETREADONLY, value ? 1 : 0, 0);
        }
    }

    public (int start, int end) Selection
    {
        get
        {
            nint result = ControlProcedures.SendMessage(Hwnd, EM.GETSEL, 0, 0);
            int start = (int)(result & 0xFFFF);
            int end = (int)((result >> 16) & 0xFFFF);
            return (start, end);
        }
        set
        {
            ControlProcedures.SendMessage(Hwnd, EM.SETSEL, (nint)value.start, (nint)value.end);
        }
    }

    public int LineCount
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, EM.GETLINECOUNT, 0, 0);
    }

    public bool Modified
    {
        get => ControlProcedures.SendMessage(Hwnd, EM.GETMODIFY, 0, 0) != 0;
        set => ControlProcedures.SendMessage(Hwnd, EM.SETMODIFY, value ? 1 : 0, 0);
    }

    public (int left, int right) Margins
    {
        get
        {
            nint result = ControlProcedures.SendMessage(Hwnd, EM.GETMARGINS, 0, 0);
            int left = (int)(result & 0xFFFF);
            int right = (int)((result >> 16) & 0xFFFF);
            return (left, right);
        }
        set
        {
            nint packed = ((nint)value.right << 16) | (ushort)value.left;
            ControlProcedures.SendMessage(Hwnd, EM.SETMARGINS, 0x0003 /* EC_LEFTMARGIN | EC_RIGHTMARGIN */, packed);
        }
    }

    public int TextLimit
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, EM.GETLIMITTEXT, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, EM.SETLIMITTEXT, (nint)value, 0);
    }

    public void SelectAll()
    {
        ControlProcedures.SendMessage(Hwnd, EM.SETSEL, 0, -1);
    }

    public void ClearSelection()
    {
        ControlProcedures.SendMessage(Hwnd, EM.SETSEL, -1, -1);
    }

    public void ReplaceSelection(string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            ControlProcedures.SendMessage(Hwnd, EM.REPLACESEL, 1, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public void ScrollCaret()
    {
        ControlProcedures.SendMessage(Hwnd, EM.SCROLLCARET, 0, 0);
    }

    public bool CanUndo()
    {
        return ControlProcedures.SendMessage(Hwnd, EM.CANUNDO, 0, 0) != 0;
    }

    public void Undo()
    {
        ControlProcedures.SendMessage(Hwnd, EM.UNDO, 0, 0);
    }

    public void EmptyUndoBuffer()
    {
        ControlProcedures.SendMessage(Hwnd, EM.EMPTYUNDOBUFFER, 0, 0);
    }

    public int GetLine(int lineIndex)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, EM.LINEFROMCHAR, (nint)lineIndex, 0);
    }

    public int GetLineLength(int charIndex)
    {
        return (int)ControlProcedures.SendMessage(Hwnd, EM.LINELENGTH, (nint)charIndex, 0);
    }

    public int GetFirstVisibleLine()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, EM.GETFIRSTVISIBLELINE, 0, 0);
    }

    public string GetLineText(int lineIndex)
    {
        int lineLength = (int)ControlProcedures.SendMessage(Hwnd, EM.LINELENGTH, 0, 0);
        if (lineLength <= 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((lineLength + 1) * 2);
        try
        {
            Marshal.WriteInt16(buffer, 0, (short)lineLength);
            ControlProcedures.SendMessage(Hwnd, EM.GETLINE, (nint)lineIndex, buffer);
            return Marshal.PtrToStringUni(buffer, lineLength) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

}
