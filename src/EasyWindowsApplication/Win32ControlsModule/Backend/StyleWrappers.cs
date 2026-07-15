using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

// ── Button Styles ──
internal readonly struct BS_PUSHBUTTON : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | BS.PUSHBUTTON;
}
internal readonly struct BS_AUTOCHECKBOX : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTOCHECKBOX;
}
internal readonly struct BS_AUTORADIOBUTTON : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.AUTORADIOBUTTON;
}
internal readonly struct BS_GROUPBOX : IButtonStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | BS.GROUPBOX;
}

// ── Edit Styles ──
internal readonly struct ES_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.AUTOHSCROLL;
}
internal readonly struct ES_MULTILINE_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.MULTILINE | ES.AUTOHSCROLL;
}
internal readonly struct ES_READONLY_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.READONLY | ES.AUTOHSCROLL;
}
internal readonly struct ES_PASSWORD_DEFAULT : IEditStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.PASSWORD | ES.AUTOHSCROLL;
}

// ── Static Styles ──
internal readonly struct SS_LEFT : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.LEFT;
}
internal readonly struct SS_CENTER : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.CENTER;
}
internal readonly struct SS_RIGHT : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.RIGHT;
}
internal readonly struct SS_SIMPLE : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.SIMPLE;
}
internal readonly struct SS_OWNERDRAW : IStaticStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SS.OWNERDRAW;
}

// ── ListBox Styles ──
internal readonly struct LBS_STANDARD : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.STANDARD;
}
internal readonly struct LBS_MULTIPLESEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.MULTIPLESEL;
}
internal readonly struct LBS_EXTENDEDSEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.EXTENDEDSEL;
}
internal readonly struct LBS_NOSEL : IListBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | WS.VSCROLL | LBS.NOSEL;
}

// ── ComboBox Styles ──
internal readonly struct CBS_DROPDOWN : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN;
}
internal readonly struct CBS_DROPDOWNLIST : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWNLIST;
}
internal readonly struct CBS_SIMPLE : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.SIMPLE;
}
internal readonly struct CBS_AUTOHSCROLL : IComboBoxStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN | CBS.AUTOHSCROLL;
}

// ── ScrollBar Styles ──
internal readonly struct SBS_HORZ : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.HORZ;
}
internal readonly struct SBS_VERT : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.VERT;
}
internal readonly struct SBS_SIZEBOX : IScrollBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBS.SIZEBOX;
}

// ── ProgressBar Styles ──
internal readonly struct PBS_SMOOTH_DEFAULT : IProgressBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | PBS.SMOOTH;
}
internal readonly struct PBS_MARQUEE_DEFAULT : IProgressBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | PBS.MARQUEE;
}

// ── DateTimePicker Styles ──
internal readonly struct DTS_SHORTDATEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.SHORTDATEFORMAT;
}
internal readonly struct DTS_LONGDATEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.LONGDATEFORMAT;
}
internal readonly struct DTS_TIMEFORMAT : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.TIMEFORMAT;
}
internal readonly struct DTS_MONTHCAL : IDateTimePickerStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | DTS.MONTHCAL;
}

// ── MonthCalendar Styles ──
internal readonly struct MCS_DAYSTATE : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.DAYSTATE;
}
internal readonly struct MCS_MULTISELECT : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.MULTISELECT;
}
internal readonly struct MCS_WEEKNUMBERS : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.WEEKNUMBERS;
}
internal readonly struct MCS_NOTODAYCIRCLE : IMonthCalendarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | MCS.NOTODAYCIRCLE;
}

// ── HotKey Styles ──
internal readonly struct HKS_DEFAULT : IHotKeyStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER;
}

// ── TrackBar Styles ──
internal readonly struct TBS_HORZ : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ;
}
internal readonly struct TBS_VERT : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.VERT;
}
internal readonly struct TBS_AUTOTICKS : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ | TBS.AUTOTICKS;
}
internal readonly struct TBS_NOTICKS : ITrackBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TBS.HORZ | TBS.NOTICKS;
}

// ── UpDown Styles ──
internal readonly struct UDS_ALIGNLEFT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.ALIGNLEFT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}
internal readonly struct UDS_ALIGNRIGHT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.ALIGNRIGHT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}
internal readonly struct UDS_SETBUDDYINT : IUpDownStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | UDS.SETBUDDYINT | UDS.ARROWKEYS | UDS.AUTOBUDDY;
}

// ── StatusBar Styles ──
internal readonly struct SBARS_DEFAULT : IStatusBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | SBARS.SIZEGRIP;
}

// ── Toolbar Styles ──
internal readonly struct TBSTYLE_FLAT : IToolbarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | TBSTYLE.FLAT | CCS.TOP;
}
internal readonly struct TBSTYLE_TRANSPARENT : IToolbarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | TBSTYLE.TRANSPARENT | CCS.TOP;
}

// ── ToolTip Styles ──
internal readonly struct TTS_DEFAULT : IToolTipStyle
{
    public static uint Value => WS.POPUP | WS_EX.TOPMOST | TTS.ALWAYSTIP;
}

// ── Header Styles ──
internal readonly struct HDS_BUTTONS : IHeaderStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | HDS.BUTTONS;
}
internal readonly struct HDS_FULLDRAG : IHeaderStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | HDS.FULLDRAG;
}

// ── TreeView Styles ──
internal readonly struct TVS_HASBUTTONS : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.HASBUTTONS | TVS.HASLINES | TVS.LINESATROOT;
}
internal readonly struct TVS_HASLINES : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.HASLINES;
}
internal readonly struct TVS_SHOWSELALWAYS : ITreeViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | TVS.SHOWSELALWAYS;
}

// ── TabControl Styles ──
internal readonly struct TCS_TABS : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.TABS;
}
internal readonly struct TCS_BUTTONS : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.BUTTONS;
}
internal readonly struct TCS_MULTILINE : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.MULTILINE;
}
internal readonly struct TCS_FIXEDWELL : ITabControlStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | TCS.FIXEDWELL;
}

// ── IPAddress Styles ──
internal readonly struct IPS_DEFAULT : IIpAddressStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP;
}

// ── ComboBoxEx Styles ──
internal readonly struct CBES_DROPDOWN : IComboBoxExStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | CBS.DROPDOWN | CBES.EX.EDITITEMS;
}

// ── ReBar Styles ──
internal readonly struct RBS_VARHEIGHT : IReBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | CCS.NORESIZE | CCS.NODIVIDER | RBS.VARIABLEHEIGHT;
}
internal readonly struct RBS_BANDBORDERS : IReBarStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | CCS.NORESIZE | CCS.NODIVIDER | RBS.BANDBORDERS;
}

// ── Link Styles ──
internal readonly struct LWS_TRANSPARENT : ILinkStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP | LWS.TRANSPARENT;
}
internal readonly struct LWS_DEFAULT : ILinkStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.TABSTOP;
}

// ── ListView Styles ──
internal readonly struct LVS_REPORT_DEFAULT : IListViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.REPORT | LVS.SHOWSELALWAYS;
}
internal readonly struct LVS_LIST_DEFAULT : IListViewStyle
{
    public static uint Value => WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.LIST | LVS.SHOWSELALWAYS;
}
