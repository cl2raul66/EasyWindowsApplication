using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

// ── Button Styles ──
public readonly struct BS_PUSHBUTTON : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | BS.PUSHBUTTON;
}
public readonly struct BS_AUTOCHECKBOX : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTOCHECKBOX;
}
public readonly struct BS_AUTORADIOBUTTON : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTORADIOBUTTON;
}
public readonly struct BS_GROUPBOX : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.GROUPBOX;
}

// ── Edit Styles ──
public readonly struct ES_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.AUTOHSCROLL;
}
public readonly struct ES_MULTILINE_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.MULTILINE | ES.AUTOHSCROLL;
}
public readonly struct ES_READONLY_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.READONLY | ES.AUTOHSCROLL;
}
public readonly struct ES_PASSWORD_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.PASSWORD | ES.AUTOHSCROLL;
}

// ── Static Styles ──
public readonly struct SS_LEFT : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.LEFT;
}
public readonly struct SS_CENTER : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.CENTER;
}
public readonly struct SS_RIGHT : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.RIGHT;
}
public readonly struct SS_SIMPLE : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.SIMPLE;
}
public readonly struct SS_OWNERDRAW : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.OWNERDRAW;
}

// ── ListBox Styles ──
public readonly struct LBS_STANDARD : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.STANDARD;
}
public readonly struct LBS_MULTIPLESEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.MULTIPLESEL;
}
public readonly struct LBS_EXTENDEDSEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.EXTENDEDSEL;
}
public readonly struct LBS_NOSEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.NOSEL;
}

// ── ComboBox Styles ──
public readonly struct CBS_DROPDOWN : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN;
}
public readonly struct CBS_DROPDOWNLIST : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWNLIST;
}
public readonly struct CBS_SIMPLE : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.SIMPLE;
}
public readonly struct CBS_AUTOHSCROLL : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN | CBS.AUTOHSCROLL;
}

// ── ScrollBar Styles ──
public readonly struct SBS_HORZ : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.HORZ;
}
public readonly struct SBS_VERT : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.VERT;
}
public readonly struct SBS_SIZEBOX : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.SIZEBOX;
}

// ── ProgressBar Styles ──
public readonly struct PBS_SMOOTH_DEFAULT : IProgressBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | PBS.SMOOTH;
}
public readonly struct PBS_MARQUEE_DEFAULT : IProgressBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | PBS.MARQUEE;
}

// ── DateTimePicker Styles ──
public readonly struct DTS_SHORTDATEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.SHORTDATEFORMAT;
}
public readonly struct DTS_LONGDATEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.LONGDATEFORMAT;
}
public readonly struct DTS_TIMEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.TIMEFORMAT;
}
public readonly struct DTS_MONTHCAL : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.MONTHCAL;
}

// ── MonthCalendar Styles ──
public readonly struct MCS_DAYSTATE : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.DAYSTATE;
}
public readonly struct MCS_MULTISELECT : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.MULTISELECT;
}
public readonly struct MCS_WEEKNUMBERS : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.WEEKNUMBERS;
}
public readonly struct MCS_NOTODAYCIRCLE : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.NOTODAYCIRCLE;
}

// ── HotKey Styles ──
public readonly struct HKS_DEFAULT : IHotKeyStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER;
}

// ── TrackBar Styles ──
public readonly struct TBS_HORZ : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ;
}
public readonly struct TBS_VERT : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.VERT;
}
public readonly struct TBS_AUTOTICKS : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ | TBS.AUTOTICKS;
}
public readonly struct TBS_NOTICKS : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ | TBS.NOTICKS;
}

// ── UpDown Styles ──
public readonly struct UDS_ALIGNLEFT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.ALIGNLEFT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}
public readonly struct UDS_ALIGNRIGHT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.ALIGNRIGHT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}
public readonly struct UDS_SETBUDDYINT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.SETBUDDYINT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}

// ── StatusBar Styles ──
public readonly struct SBARS_DEFAULT : IStatusBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBARS.SIZEGRIP;
}

// ── Toolbar Styles ──
public readonly struct TBSTYLE_FLAT : IToolbarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | TBSTYLE.FLAT | CCS.TOP;
}
public readonly struct TBSTYLE_TRANSPARENT : IToolbarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | TBSTYLE.TRANSPARENT | CCS.TOP;
}

// ── ToolTip Styles ──
public readonly struct TTS_DEFAULT : IToolTipStyle
{
    public static uint Value => WS.POPUP | WS_EX.TOPMOST | TTS.ALWAYSTIP;
}

// ── Header Styles ──
public readonly struct HDS_BUTTONS : IHeaderStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | HDS.BUTTONS;
}
public readonly struct HDS_FULLDRAG : IHeaderStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | HDS.FULLDRAG;
}

// ── TreeView Styles ──
public readonly struct TVS_HASBUTTONS : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.HASBUTTONS | TVS.HASLINES | TVS.LINESATROOT;
}
public readonly struct TVS_HASLINES : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.HASLINES;
}
public readonly struct TVS_SHOWSELALWAYS : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.SHOWSELALWAYS;
}

// ── TabControl Styles ──
public readonly struct TCS_TABS : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.TABS;
}
public readonly struct TCS_BUTTONS : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.BUTTONS;
}
public readonly struct TCS_MULTILINE : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.MULTILINE;
}
public readonly struct TCS_FIXEDWELL : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.FIXEDWELL;
}

// ── IPAddress Styles ──
public readonly struct IPS_DEFAULT : IIpAddressStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP;
}

// ── ComboBoxEx Styles ──
public readonly struct CBES_DROPDOWN : IComboBoxExStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN | CBES.EX.EDITITEMS;
}

// ── ReBar Styles ──
public readonly struct RBS_VARHEIGHT : IReBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | CCS.NORESIZE | CCS.NODIVIDER | RBS.VARIABLEHEIGHT;
}
public readonly struct RBS_BANDBORDERS : IReBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | CCS.NORESIZE | CCS.NODIVIDER | RBS.BANDBORDERS;
}

// ── Link Styles ──
public readonly struct LWS_TRANSPARENT : ILinkStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | LWS.TRANSPARENT;
}
public readonly struct LWS_DEFAULT : ILinkStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP;
}

// ── ListView Styles ──
public readonly struct LVS_REPORT_DEFAULT : IListViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.REPORT | LVS.SHOWSELALWAYS;
}
public readonly struct LVS_LIST_DEFAULT : IListViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.LIST | LVS.SHOWSELALWAYS;
}
