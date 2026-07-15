using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal readonly record struct Win32ControlInfo(string ClassName, uint DefaultStyle);

internal static class Win32ControlMap
{
    public static Win32ControlInfo Get<T>() where T : ControlBase<T>, new()
    {
        return typeof(T).Name switch
        {
            nameof(Button) => new(WC.BUTTON, WS.CHILD | WS.VISIBLE | BS.PUSHBUTTON),
            nameof(Edit) => new(WC.EDIT, WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.AUTOHSCROLL),
            "Progress" => new(WC.PROGRESS, WS.CHILD | WS.VISIBLE),
            "ListView" => new(WC.LISTVIEW, WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.REPORT | LVS.SHOWSELALWAYS),
            "Label" => new(WC.STATIC, WS.CHILD | WS.VISIBLE | SS.LEFT),
            "CheckBox" => new(WC.BUTTON, WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTOCHECKBOX),
            "RadioButton" => new(WC.BUTTON, WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTORADIOBUTTON),
            "GroupBox" => new(WC.BUTTON, WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.GROUPBOX),
            "ListBox" => new(WC.LISTBOX, WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.STANDARD),
            "ComboBox" => new(WC.COMBOBOX, WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN),
            "ComboBoxEx" => new(WC.COMBOBOXEX, WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN),
            "ScrollBar" => new(WC.SCROLLBAR, WS.CHILD | WS.VISIBLE | SBS.HORZ),
            "DateTimePicker" => new(WC.DATETIMEPICK, WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.SHORTDATEFORMAT),
            "MonthCalendar" => new(WC.MONTHCAL, WS.CHILD | WS.VISIBLE | WS.TABSTOP),
            "HotKey" => new(WC.HOTKEY, WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER),
            "TrackBar" => new(WC.TRACKBAR, WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ),
            "UpDown" => new(WC.UPDOWN, WS.CHILD | WS.VISIBLE | UDS.ALIGNLEFT | UDS.ARROWKEYS | UDS.AUTOBUDDY),
            "StatusBar" => new(WC.STATUSBAR, WS.CHILD | WS.VISIBLE | SBARS.SIZEGRIP),
            "Toolbar" => new(WC.TOOLBAR, WS.CHILD | WS.VISIBLE | TBSTYLE.FLAT | CCS.TOP),
            "ToolTip" => new(WC.TOOLTIP, WS.POPUP | WS_EX.TOPMOST | TTS.ALWAYSTIP),
            "Header" => new(WC.HEADER, WS.CHILD | WS.VISIBLE | WS.BORDER | HDS.BUTTONS),
            "TreeView" => new(WC.TREEVIEW, WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.HASBUTTONS | TVS.HASLINES | TVS.LINESATROOT),
            "TabControl" => new(WC.TABCONTROL, WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.TABS),
            "IpAddress" => new(WC.IPADDRESS, WS.CHILD | WS.VISIBLE | WS.TABSTOP),
            "ReBar" => new(WC.REBAR, WS.CHILD | WS.VISIBLE | CCS.NORESIZE | CCS.NODIVIDER | RBS.VARIABLEHEIGHT),
            "Link" => new(WC.LINK, WS.CHILD | WS.VISIBLE | WS.TABSTOP),
            _ => new(WC.BUTTON, WS.CHILD | WS.VISIBLE),
        };
    }
}
