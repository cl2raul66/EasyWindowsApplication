namespace EasyWindowsApplication.Win32ControlsModule.Backend;

// ── WC (Window Class names) ──
internal static class WC
{
    internal const string BUTTON    = "BUTTON";
    internal const string EDIT      = "EDIT";
    internal const string STATIC    = "STATIC";
    internal const string LISTBOX   = "LISTBOX";
    internal const string COMBOBOX  = "COMBOBOX";
    internal const string SCROLLBAR = "SCROLLBAR";
    internal const string PROGRESS  = "msctls_progress32";
    internal const string LISTVIEW  = "SysListView32";
    internal const string TREEVIEW  = "SysTreeView32";
    internal const string TABCONTROL = "SysTabControl32";
    internal const string DATETIMEPICK = "SysDateTimePick32";
    internal const string MONTHCAL  = "SysMonthCal32";
    internal const string HOTKEY    = "msctls_hotkey32";
    internal const string TRACKBAR  = "msctls_trackbar32";
    internal const string UPDOWN    = "msctls_updown32";
    internal const string STATUSBAR = "msctls_statusbar32";
    internal const string TOOLBAR   = "ToolbarWindow32";
    internal const string TOOLTIP   = "tooltips_class32";
    internal const string HEADER    = "SysHeader32";
    internal const string IPADDRESS = "SysIPAddress32";
    internal const string COMBOBOXEX = "ComboBoxEx32";
    internal const string REBAR     = "ReBarWindow32";
    internal const string LINK      = "SysLink";
}

// ── BS (Button Styles) ──
internal static class BS
{
    internal const uint PUSHBUTTON    = 0x00000000;
    internal const uint DEFPUSHBUTTON = 0x00000001;
    internal const uint CHECKBOX      = 0x00000002;
    internal const uint AUTOCHECKBOX  = 0x00000003;
    internal const uint AUTORADIOBUTTON = 0x00000004;
    internal const uint GROUPBOX      = 0x00000007;
    internal const uint LEFTTEXT      = 0x00000020;
    internal const uint TOPSTYLE      = 0x00000008;
    internal const uint FLAT          = 0x00002000;
    internal const uint OWNERDRAW     = 0x0000000B;
    internal const uint TYPEMASK      = 0x0000000F;
    internal const uint MULTILINE     = 0x00002000;
}

// ── ES (Edit Styles) ──
internal static class ES
{
    internal const uint LEFT         = 0x00000000;
    internal const uint CENTER       = 0x00000001;
    internal const uint RIGHT        = 0x00000002;
    internal const uint MULTILINE    = 0x00000004;
    internal const uint LOWERCASE    = 0x00000010;
    internal const uint UPPERCASE    = 0x00000020;
    internal const uint PASSWORD     = 0x00000020;
    internal const uint AUTOVSCROLL  = 0x00000040;
    internal const uint AUTOHSCROLL  = 0x00000080;
    internal const uint WANTRETURN   = 0x00001000;
    internal const uint NUMBER       = 0x00002000;
    internal const uint READONLY     = 0x00000800;
    internal const uint WANTTAB      = 0x00080000;
}

// ── SS (Static Styles) ──
internal static class SS
{
    internal const uint LEFT         = 0x00000000;
    internal const uint CENTER       = 0x00000001;
    internal const uint RIGHT        = 0x00000002;
    internal const uint ICON         = 0x00000003;
    internal const uint BLACKRECT    = 0x00000004;
    internal const uint GRAYRECT     = 0x00000005;
    internal const uint WHITERECT    = 0x00000006;
    internal const uint BLACKFRAME   = 0x00000007;
    internal const uint GRAYFRAME    = 0x00000008;
    internal const uint WHITEFRAME   = 0x00000009;
    internal const uint USERITEM     = 0x0000000A;
    internal const uint SIMPLE       = 0x0000000B;
    internal const uint LEFTNOWORDWRAP = 0x0000000C;
    internal const uint OWNERDRAW    = 0x0000000D;
    internal const uint BITMAP       = 0x0000000E;
    internal const uint ENHMETAFILE = 0x0000000F;
    internal const uint HREDRAW      = 0x00010000;
    internal const uint VREDRAW      = 0x00020000;
    internal const uint AUTOSize     = 0x00000040;
    internal const uint REALSIZECONTROL = 0x00000040;
    internal const uint TYPEMASK     = 0x0000001F;
    internal const uint NOPREFIX     = 0x00000080;
    internal const uint NOTIFY       = 0x00000100;
    internal const uint SETCURSEL    = 0x00000200;
}

// ── LBS (ListBox Styles) ──
internal static class LBS
{
    internal const uint ERRORALERT     = 0x00000000;
    internal const uint OWNERDRAWFIXED = 0x00000010;
    internal const uint OWNERDRAWVARIABLE = 0x00000020;
    internal const uint HASSTRINGS     = 0x00000040;
    internal const uint SORT           = 0x00000002;
    internal const uint NOSEL          = 0x00004000;
    internal const uint MULTIPLESEL    = 0x00000008;
    internal const uint STANDARD       = 0x00A00003;
    internal const uint EXTENDEDSEL    = 0x00000200;
    internal const uint DISABLENOSCROLL = 0x00002000;
    internal const uint NODATA         = 0x00010000;
    internal const uint NOINTEGRALHEIGHT = 0x00000100;
    internal const uint WANTKEYBOARDINPUT = 0x00000400;
    internal const uint COMBOBOX       = 0x00000080;
    internal const uint STYLEMASK      = 0x00A000BF;
}

// ── CBS (ComboBox Styles) ──
internal static class CBS
{
    internal const uint SIMPLE           = 0x00000001;
    internal const uint DROPDOWN         = 0x00000002;
    internal const uint DROPDOWNLIST     = 0x00000003;
    internal const uint OWNERDRAWFIXED   = 0x00000010;
    internal const uint OWNERDRAWVARIABLE = 0x00000020;
    internal const uint AUTOHSCROLL      = 0x00000040;
    internal const uint OEMCONVERT       = 0x00000080;
    internal const uint SORT             = 0x00000100;
    internal const uint HASSTRINGS       = 0x00000200;
    internal const uint NOINTEGRALHEIGHT = 0x00000400;
    internal const uint DISABLENOSCROLL  = 0x00000800;
    internal const uint EXACTSEL         = 0x00004000;
    internal const uint AUTOLOCAL        = 0x00004000;
}

// ── SBS (ScrollBar Styles) ──
internal static class SBS
{
    internal const uint HORZ             = 0x00000000;
    internal const uint VERT             = 0x00000001;
    internal const uint SIZEBOX          = 0x00000002;
    internal const uint SIZEBOXTOPLEFT   = 0x00000000;
    internal const uint SIZEBOXBOTTOMRIGHT = 0x00000004;
    internal const uint SIZEGRIP         = 0x00000008;
}

// ── PBS (ProgressBar Styles) ──
internal static class PBS
{
    internal const uint SMOOTH  = 0x00000001;
    internal const uint VERTICAL = 0x00000002;
    internal const uint MARQUEE = 0x00000008;
}

// ── PBM (ProgressBar Messages) ──
internal static class PBM
{
    internal const uint SETRANGE    = 0x0401;
    internal const uint SETPOS      = 0x0402;
    internal const uint DELTA       = 0x0403;
    internal const uint SETRANGE32  = 0x0406;
    internal const uint GETPOS      = 0x0408;
    internal const uint SETBARCOLOR = 0x0409;
    internal const uint SETBKCOLOR  = 0x040A;
}

// ── DTS (DateTimePicker Styles) ──
internal static class DTS
{
    internal const uint UPDOWN           = 0x00000001;
    internal const uint SHOWNONE         = 0x00000002;
    internal const uint SHORTDATEFORMAT  = 0x00000000;
    internal const uint LONGDATEFORMAT   = 0x00000004;
    internal const uint TIMEFORMAT       = 0x00000008;
    internal const uint DATEBOX          = 0x00000010;
    internal const uint SHORTDATECENTURYFORMAT = 0x0000000C;
    internal const uint MONTHCAL         = 0x00000020;
    internal const uint RIGHTTOLEFT      = 0x00000040;
}

// ── DTM (DateTimePicker Messages) ──
internal static class DTM
{
    internal const uint SETSYSTEMTIME = 0x1002;
    internal const uint GETSYSTEMTIME = 0x1003;
    internal const uint SETRANGE      = 0x1004;
    internal const uint GETRANGE      = 0x1005;
    internal const uint SETFORMAT     = 0x1005;
    internal const uint GETMCCOLOR    = 0x1007;
    internal const uint SETMCCOLOR    = 0x1008;
    internal const uint SETMCFONT     = 0x1009;
}

// ── MCS (MonthCalendar Styles) ──
internal static class MCS
{
    internal const uint DAYSTATE       = 0x0001;
    internal const uint MULTISELECT    = 0x0002;
    internal const uint WEEKNUMBERS    = 0x0004;
    internal const uint NOTODAYCIRCLE  = 0x0008;
    internal const uint NOTODAY        = 0x0010;
    internal const uint NOWEEKBAR      = 0x0020;
    internal const uint NOTRAILINGSELECTEDDT = 0x0040;
    internal const uint SHORTDAYSOFWEEK = 0x0080;
    internal const uint NOSELCHANGE_NOTIFY = 0x0100;
}

// ── MCM (MonthCalendar Messages) ──
internal static class MCM
{
    internal const uint FIRST           = 0x1000;
    internal const uint GETCURSEL       = FIRST + 1;
    internal const uint SETCURSEL       = FIRST + 2;
    internal const uint GETSELRANGE     = FIRST + 3;
    internal const uint SETSELRANGE     = FIRST + 4;
    internal const uint GETMONTHRANGE   = FIRST + 5;
    internal const uint SETDAYSTATE     = FIRST + 6;
    internal const uint GETMAXTODAYRECT = FIRST + 7;
    internal const uint SETMAXTODAYRECT = FIRST + 8;
    internal const uint GETRANGE        = FIRST + 9;
    internal const uint SETRANGE        = FIRST + 10;
    internal const uint GETMONTHCOLOR   = FIRST + 9;
    internal const uint SETMONTHCOLOR   = FIRST + 10;
}

// ── HKM (HotKey Messages) ──
internal static class HKM
{
    internal const uint SETHOTKEY  = 0x0401;
    internal const uint GETHOTKEY  = 0x0402;
    internal const uint RULES      = 0x0403;
}

// ── TBS (TrackBar Styles) ──
internal static class TBS
{
    internal const uint AUTOTICKS    = 0x0001;
    internal const uint VERT         = 0x0002;
    internal const uint HORZ         = 0x0000;
    internal const uint TOP          = 0x0004;
    internal const uint BOTTOM       = 0x0008;
    internal const uint LEFT         = 0x0004;
    internal const uint RIGHT        = 0x0008;
    internal const uint NOTICKS      = 0x0010;
    internal const uint ENABLESELRANGE = 0x0020;
    internal const uint FIXEDLENGTH  = 0x0040;
    internal const uint RETHUMB      = 0x0080;
    internal const uint TOOLTIPS     = 0x0100;
    internal const uint REVERSED     = 0x0200;
    internal const uint DOWNISLEFT   = 0x0400;
    internal const uint NOTIFYBEFOREMOVE = 0x0800;
    internal const uint TRANSPARENTBKGND = 0x1000;
}

// ── TBM (TrackBar Messages) ──
internal static class TBM
{
    internal const uint FIRST         = 0x0400;
    internal const uint SETRANGE      = FIRST + 5;
    internal const uint SETRANGEMIN   = FIRST + 7;
    internal const uint SETRANGEMAX   = FIRST + 6;
    internal const uint SETTICFREQ    = FIRST + 20;
    internal const uint SETPOS        = FIRST + 5;
    internal const uint GETPOS        = FIRST + 0;
    internal const uint GETRANGEMIN   = FIRST + 3;
    internal const uint GETRANGEMAX   = FIRST + 4;
    internal const uint SETLINESIZE   = FIRST + 17;
    internal const uint GETLINESIZE   = FIRST + 18;
    internal const uint SETPAGEsize   = FIRST + 21;
    internal const uint GETPAGEsize   = FIRST + 22;
    internal const uint SETTHUMBLENGTH = FIRST + 27;
    internal const uint GETTHUMBLENGTH = FIRST + 28;
    internal const uint SETSEL        = FIRST + 11;
    internal const uint GETSELSTART   = FIRST + 9;
    internal const uint GETSEL        = FIRST + 12;
}

// ── UDS (UpDown Styles) ──
internal static class UDS
{
    internal const uint WRAP           = 0x00000001;
    internal const uint SETBUDDYINT    = 0x00000002;
    internal const uint ALIGNLEFT      = 0x00000008;
    internal const uint ALIGNRIGHT     = 0x00000004;
    internal const uint AUTOBUDDY      = 0x00000010;
    internal const uint ARROWKEYS      = 0x00000020;
    internal const uint HORZ           = 0x00000040;
    internal const uint NOTHOUSANDS    = 0x00000080;
    internal const uint HOTTRACK       = 0x00000100;
    internal const uint DECIMAL        = 0x00000200;
    internal const uint BINARY         = 0x00000400;
}

// ── UDM (UpDown Messages) ──
internal static class UDM
{
    internal const uint SETRANGE    = 0x0465;
    internal const uint GETRANGE    = 0x0466;
    internal const uint SETPOS      = 0x0467;
    internal const uint GETPOS      = 0x0468;
    internal const uint SETBUDDY    = 0x0469;
    internal const uint GETBUDDY    = 0x046A;
    internal const uint SETACCEL    = 0x046B;
    internal const uint GETACCEL    = 0x046C;
    internal const uint SETBASE     = 0x046D;
    internal const uint GETBASE     = 0x046E;
    internal const uint SETPOS32    = 0x0471;
    internal const uint GETPOS32    = 0x0472;
}

// ── SB (StatusBar) ──
internal static class SB
{
    internal const uint SETTEXTA     = 0x0401;
    internal const uint SETTEXTW     = 0x040B;
    internal const uint GETTEXTA     = 0x0402;
    internal const uint GETTEXTW     = 0x040D;
    internal const uint GETTEXTLENGTHA = 0x0403;
    internal const uint GETTEXTLENGTHW = 0x040C;
    internal const uint PARTS        = 0x0400;
    internal const uint GETBORDERS   = 0x0407;
    internal const uint SETMINHEIGHT = 0x0408;
    internal const uint SETSimple    = 0x0418;
    internal const uint GETRECT      = 0x040A;
    internal const uint ISSIMPLE     = 0x0419;
    internal const uint SETICON      = 0x0414;
    internal const uint SETTIPTEXTA  = 0x0410;
    internal const uint SETTIPTEXTW  = 0x0411;
}

// ── SBARS (StatusBar Styles) ──
internal static class SBARS
{
    internal const uint SIZEGRIP = 0x0100;
    internal const uint TOOLTIPS = 0x0800;
    internal const uint OWNERDRAW = 0x0400;
    internal const uint SIMPLE = 0x0001;
}

// ── TBSTYLE (Toolbar Styles) ──
internal static class TBSTYLE
{
    internal const uint FLAT         = 0x0400;
    internal const uint LIST         = 0x0800;
    internal const uint CUSTOMERASE = 0x2000;
    internal const uint REGISTERDROP = 0x4000;
    internal const uint TRANSPARENT  = 0x8000;
    internal const uint WRAPABLE     = 0x0200;
    internal const uint ALTDRAG      = 0x0400;
    internal const uint BUTTON       = 0x0000;
    internal const uint SEP          = 0x0001;
    internal const uint CHECK        = 0x0002;
    internal const uint GROUP        = 0x0004;
    internal const uint CHECKGROUP   = 0x0006;
    internal const uint DROPDOWN     = 0x0008;
    internal const uint AUTOSIZE     = 0x0010;
    internal const uint NOPREFIX     = 0x0020;
    internal const uint WHOLEDROPDOWN = 0x0080;
}

// ── CCS (Common Control Styles) ──
internal static class CCS
{
    internal const uint TOP          = 0x00000000;
    internal const uint LEFT         = 0x00000001;
    internal const uint RIGHT        = 0x00000003;
    internal const uint VISIBLE      = 0x00000080;
    internal const uint NORESIZE     = 0x00000010;
    internal const uint NOPARENTALIGN = 0x00000008;
    internal const uint NOREPOSITION = 0x00000200;
    internal const uint ADJUSTABLE   = 0x00000020;
    internal const uint FLOAT        = 0x00000400;
    internal const uint NODIVIDER    = 0x00000040;
}

// ── TTS (ToolTip Styles) ──
internal static class TTS
{
    internal const uint ALWAYSTIP    = 0x0001;
    internal const uint NOPREFIX     = 0x0002;
    internal const uint NOANIMATE    = 0x0010;
    internal const uint NOFADE       = 0x0020;
    internal const uint CLOSEBUTTON  = 0x0080;
}

// ── TTM (ToolTip Messages) ──
internal static class TTM
{
    internal const uint FIRST        = 0x0400;
    internal const uint ADDTOOL      = FIRST + 4;
    internal const uint DELTOOL      = FIRST + 5;
    internal const uint NEWTOOLRECT  = FIRST + 9;
    internal const uint GETTOOLINFO  = FIRST + 8;
    internal const uint SETTOOLINFO  = FIRST + 9;
    internal const uint HITTEST      = FIRST + 10;
    internal const uint GETTEXT      = FIRST + 11;
    internal const uint UPDATETIPTEXT = FIRST + 12;
    internal const uint GETTOOLCOUNT = FIRST + 13;
    internal const uint ACTIVE       = FIRST + 2;
    internal const uint DEACTIVATE   = FIRST + 3;
    internal const uint POP          = FIRST + 14;
    internal const uint CLOSE        = FIRST + 15;
}

// ── HDS (Header Styles) ──
internal static class HDS
{
    internal const uint BUTTONS       = 0x00000001;
    internal const uint HOTTRACK      = 0x00000004;
    internal const uint HIDDEN        = 0x00000008;
    internal const uint FULLDRAG      = 0x00000080;
    internal const uint FILTERBAR     = 0x00000100;
    internal const uint FLAT          = 0x00000020;
    internal const uint NOSIZING      = 0x00000200;
    internal const uint OVERFLOW      = 0x00000400;
}

// ── HDM (Header Messages) ──
internal static class HDM
{
    internal const uint FIRST            = 0x1200;
    internal const uint INSERTITEMA      = FIRST + 1;
    internal const uint INSERTITEMW      = FIRST + 10;
    internal const uint DELETEITEM       = FIRST + 2;
    internal const uint GETITEMA         = FIRST + 3;
    internal const uint GETITEMW         = FIRST + 11;
    internal const uint SETITEMA         = FIRST + 4;
    internal const uint SETITEMW         = FIRST + 12;
    internal const uint LAYOUT           = FIRST + 5;
    internal const uint GETITEMCOUNT     = FIRST + 0;
    internal const uint SETIMAGELIST     = FIRST + 8;
    internal const uint GETIMAGELIST     = FIRST + 9;
}

// ── TVS (TreeView Styles) ──
internal static class TVS
{
    internal const uint HASBUTTONS      = 0x0001;
    internal const uint HASLINES        = 0x0002;
    internal const uint LINESATROOT     = 0x0004;
    internal const uint EDITLABELS      = 0x0008;
    internal const uint DISABLEDRAGDROP = 0x0010;
    internal const uint SHOWSELALWAYS   = 0x0020;
    internal const uint RTLREADING      = 0x0040;
    internal const uint CHECKBOXES      = 0x0100;
    internal const uint TRACKSELECT     = 0x0200;
    internal const uint FULLROWSELECT   = 0x1000;
    internal const uint HASINFOTIP      = 0x0800;
    internal const uint JAVASCRIPT      = 0x4000;
    internal const uint NOATSCROLL      = 0x4000;
    internal const uint NONEVENINSERT   = 0x0080;
    internal const uint ISCROLLABLE     = 0x2000;
}

// ── TVM (TreeView Messages) ──
internal static class TVM
{
    internal const uint FIRST          = 0x1100;
    internal const uint INSERTITEMA    = FIRST + 0;
    internal const uint INSERTITEMW    = FIRST + 67;
    internal const uint DELETEITEM     = FIRST + 1;
    internal const uint EXPAND         = FIRST + 2;
    internal const uint GETINDENT      = FIRST + 5;
    internal const uint SETINDENT      = FIRST + 6;
    internal const uint GETIMAGELIST   = FIRST + 8;
    internal const uint SETIMAGELIST   = FIRST + 9;
    internal const uint GETNEXTITEM    = FIRST + 9;
    internal const uint SELECTITEM     = FIRST + 11;
    internal const uint GETITEMA       = FIRST + 12;
    internal const uint GETITEMW       = FIRST + 75;
    internal const uint SETITEMA       = FIRST + 13;
    internal const uint SETITEMW       = FIRST + 76;
    internal const uint EDITLABELA     = FIRST + 14;
    internal const uint EDITLABELW     = FIRST + 79;
    internal const uint GETEDITCONTROL = FIRST + 16;
    internal const uint GETVISIBLECOUNT = FIRST + 17;
    internal const uint HITTEST        = FIRST + 18;
    internal const uint GETISEARCHSTRINGA = FIRST + 23;
    internal const uint GETISEARCHSTRINGW = FIRST + 64;
    internal const uint SETITEMHEIGHT   = FIRST + 27;
    internal const uint GETITEMHEIGHT   = FIRST + 28;
    internal const uint SETBKCOLOR      = FIRST + 29;
    internal const uint SETTEXTCOLOR    = FIRST + 30;
    internal const uint GETBKCOLOR      = FIRST + 31;
    internal const uint GETTEXTCOLOR    = FIRST + 32;
    internal const uint SETSCROLLTIME   = FIRST + 33;
    internal const uint GETSCROLLTIME   = FIRST + 34;
    internal const uint SETINSERTMARK   = FIRST + 66;
    internal const uint SETUNICODEFORMAT = FIRST + 77;
    internal const uint GETUNICODEFORMAT = FIRST + 78;
}

// ── TVGN (TreeView GetNext constants) ──
internal static class TVGN
{
    internal const uint NEXT         = 0x0001;
    internal const uint PREVIOUS     = 0x0002;
    internal const uint FIRSTVISIBLE = 0x0005;
    internal const uint NEXTVISIBLE = 0x0006;
    internal const uint PREVIOUSVISIBLE = 0x0007;
    internal const uint PARENT       = 0x0003;
    internal const uint CHILD        = 0x0004;
    internal const uint DROPHILITE   = 0x0008;
    internal const uint CARET        = 0x0009;
    internal const uint LASTVISIBLE = 0x000A;
    internal const uint NEXTSELECTED = 0x000B;
    internal const uint NOWEBCOLOR   = 0x000C;
    internal const uint NOSTATE      = 0x000D;
}

// ── TCS (TabControl Styles) ──
internal static class TCS
{
    internal const uint SCROLLBUTTONS = 0x0001;
    internal const uint HOTTRACK      = 0x0002;
    internal const uint DROPDOWN      = 0x0080;
    internal const uint MULTISELECT   = 0x0004;
    internal const uint FORCELEFTLEFT = 0x0010;
    internal const uint FORCERIGHTLEFT = 0x0020;
    internal const uint FORCELEFTTOP  = 0x0040;
    internal const uint FORCERIGHTTOP = 0x0080;
    internal const uint BUTTONS       = 0x1000;
    internal const uint MULTILINE     = 0x0002;
    internal const uint FIXEDWELL     = 0x0000;
    internal const uint RAGGEDRIGHT   = 0x0000;
    internal const uint FOCUSONBUTTONDOWN = 0x1000;
    internal const uint BOTTOM        = 0x0002;
    internal const uint TABS          = 0x0000;
}

// ── TCM (TabControl Messages) ──
internal static class TCM
{
    internal const uint FIRST          = 0x1300;
    internal const uint INSERTITEMA    = FIRST + 1;
    internal const uint INSERTITEMW    = FIRST + 62;
    internal const uint DELETEALLITEMS = FIRST + 9;
    internal const uint GETITEMRECT    = FIRST + 10;
    internal const uint SETCURSEL      = FIRST + 12;
    internal const uint GETCURSEL      = FIRST + 11;
    internal const uint SETIMAGELIST   = FIRST + 3;
    internal const uint GETIMAGELIST   = FIRST + 2;
    internal const uint ADJUSTRECT     = FIRST + 40;
    internal const uint SETITEMA       = FIRST + 6;
    internal const uint SETITEMW       = FIRST + 61;
    internal const uint GETITEMA       = FIRST + 5;
    internal const uint GETITEMW       = FIRST + 60;
    internal const uint SETMINTABWIDTH = FIRST + 49;
    internal const uint DeselectAll    = FIRST + 50;
    internal const uint HIGHLIGHTFIRSTITEM = FIRST + 21;
}

// ── IPM (IPAddress Messages) ──
internal static class IPM
{
    internal const uint CLEARADDRESS  = 0x0464;
    internal const uint SETADDRESS    = 0x0465;
    internal const uint GETADDRESS    = 0x0466;
    internal const uint SETRANGE      = 0x0467;
    internal const uint SETFOCUS      = 0x0468;
    internal const uint ISBLANK       = 0x0469;
}

// ── CBES (ComboBoxEx Styles/Messages) ──
internal static class CBES
{
    internal const uint EXNOEDIT     = 0x0001;
    internal const uint EXNOSIZING  = 0x0002;
    internal const uint EXNOCASE    = 0x0004;

    internal static class EX
    {
        internal const uint EDITITEMS = 0x00000001;
    }

    internal static class CBEM
    {
        internal const uint FIRST         = 0x1500;
        internal const uint INSERTITEMA   = FIRST + 1;
        internal const uint INSERTITEMW   = FIRST + 11;
        internal const uint SETITEMA      = FIRST + 2;
        internal const uint SETITEMW      = FIRST + 12;
        internal const uint GETITEMA      = FIRST + 3;
        internal const uint GETITEMW      = FIRST + 13;
        internal const uint DELETEITEM    = FIRST + 4;
        internal const uint GETCOMBOCONTROL = FIRST + 6;
        internal const uint GETEDITCONTROL = FIRST + 7;
        internal const uint SETIMAGELIST  = FIRST + 8;
        internal const uint GETIMAGELIST  = FIRST + 9;
        internal const uint SETEXTENDEDSTYLE = FIRST + 14;
        internal const uint GETEXTENDEDSTYLE = FIRST + 15;
       internal const uint SETUNICODEFORMAT = FIRST + 20;
        internal const uint GETUNICODEFORMAT = FIRST + 21;
    }
}

// ── RBS (ReBar Styles) ──
internal static class RBS
{
    internal const uint CHILDEDGE     = 0x0001;
    internal const uint FIXEDBMP      = 0x0004;
    internal const uint VARIABLEHEIGHT = 0x0008;
    internal const uint BANDBORDERS   = 0x0010;
    internal const uint FIXEDORDER    = 0x0020;
    internal const uint REGISTERDROP  = 0x0040;
    internal const uint AUTOSIZE      = 0x0080;
    internal const uint DBLCLKTOGGLE  = 0x0100;
}

// ── RB (ReBar Messages) ──
internal static class RB
{
    internal const uint FIRST         = 0x0400;
    internal const uint INSERTBANDA   = FIRST + 1;
    internal const uint INSERTBANDW   = FIRST + 10;
    internal const uint SETBANDINFOA  = FIRST + 6;
    internal const uint SETBANDINFOW  = FIRST + 11;
    internal const uint GETBANDINFO   = FIRST + 2;
    internal const uint DELETEBAND    = FIRST + 2;
    internal const uint GETBANDCOUNT  = FIRST + 12;
    internal const uint GETROWCOUNT   = FIRST + 13;
    internal const uint GETROWHEIGHT  = FIRST + 14;
    internal const uint SETBKCOLOR    = FIRST + 17;
    internal const uint GETBKCOLOR    = FIRST + 18;
    internal const uint SETTEXTCOLOR  = FIRST + 19;
    internal const uint GETTEXTCOLOR  = FIRST + 20;
    internal const uint BANDCOLORS    = FIRST + 22;
    internal const uint GETBANDMARGINS = FIRST + 23;
    internal const uint SETBANDMARGINS = FIRST + 24;
    internal const uint SETPARENT     = FIRST + 25;
}

// ── LWS (Link Styles) ──
internal static class LWS
{
    internal const uint TRANSPARENT   = 0x00000001;
    internal const uint TRANSPARENTBACKGROUND = 0x00000002;
}

// ── LM (Link Messages) ──
internal static class LM
{
    internal const uint GETITEMID    = 0x0700;
    internal const uint GETITEM      = 0x0701;
}

// ── LVM (ListView Messages) ──
internal static class LVM
{
    internal const uint FIRST = 0x1000;
    internal const uint GETITEMCOUNT      = FIRST + 4;
    internal const uint GETEXTENDEDLISTVIEWSTYLE = FIRST + 55;
    internal const uint INSERTITEMW       = FIRST + 77;
    internal const uint SETITEMW          = FIRST + 76;
    internal const uint GETITEMW          = FIRST + 75;
    internal const uint DELETEITEM        = FIRST + 8;
    internal const uint DELETEALLITEMS    = FIRST + 9;
    internal const uint GETSELECTEDCOUNT  = FIRST + 50;
    internal const uint SETEXTENDEDLISTVIEWSTYLE = FIRST + 54;
    internal const uint INSERTCOLUMNW     = FIRST + 97;
    internal const uint SETCOLUMNWIDTH    = FIRST + 30;
    internal const uint SETITEMSTATE      = FIRST + 43;
    internal const uint GETITEMSTATE      = FIRST + 44;
    internal const uint SETITEMTEXTW      = FIRST + 116;
}

// ── LVS (ListView Styles) ──
internal static class LVS
{
    internal const uint REPORT       = 0x0001;
    internal const uint LIST         = 0x0003;
    internal const uint SMALLICON    = 0x0002;
    internal const uint ICON         = 0x0000;
    internal const uint SHOWSELALWAYS = 0x0008;
    internal const uint SINGLESEL    = 0x0004;
    internal const uint NOSORTHEADER = 0x8000;
    internal const uint OWNERDRAWFIXED = 0x0400;
    internal const uint SHAREIMAGELISTS = 0x0040;
    internal const uint NOLABELWRAP  = 0x0080;
    internal const uint EDITLABELS   = 0x0200;
    internal const uint NOSCROLL     = 0x2000;
    internal const uint ALIGNTOP     = 0x0000;
    internal const uint ALIGNLEFT    = 0x0800;
    internal const uint ALIGNMASK    = 0x0800;
    internal const uint OWNERDATA    = 0x1000;
}

// ── LVS_EX (ListView Extended Styles) ──
internal static class LVS_EX
{
    internal const uint FULLROWSELECT  = 0x00000020;
    internal const uint DOUBLEBUFFER   = 0x00010000;
    internal const uint HEADERINALLVIEWS = 0x00020000;
    internal const uint GRIDLINES      = 0x00000001;
    internal const uint CHECKBOXES     = 0x00000004;
    internal const uint TRACKSELECT    = 0x00000008;
    internal const uint HEADERDRAGDROP = 0x00000010;
    internal const uint INFOTIP        = 0x00000040;
    internal const uint UNDERLINEHOT   = 0x00000080;
    internal const uint UNDERLINECOLD  = 0x00000100;
    internal const uint REGIONAL       = 0x00000200;
    internal const uint BORDERSELECT   = 0x00008000;
    internal const uint MULTIPLEWORKAREAS = 0x00000002;
}

// ── LVCF (ListView Column Formats) ──
internal static class LVCF
{
    internal const uint TEXT  = 0x0001;
    internal const uint WIDTH = 0x0002;
    internal const uint FMT  = 0x0004;
    internal const uint IMAGE = 0x0008;
    internal const uint ORDER = 0x0010;
}

// ── LVIF (ListView Item Flags) ──
internal static class LVIF
{
    internal const uint TEXT     = 0x0001;
    internal const uint IMAGE    = 0x0002;
    internal const uint PARAM    = 0x0004;
    internal const uint STATE    = 0x0008;
    internal const uint INDENT   = 0x0010;
    internal const uint NORECOMPUTE = 0x0800;
}

// ── LVIS (ListView Item State) ──
internal static class LVIS
{
    internal const uint SELECTED    = 0x0002;
    internal const uint FOCUSED     = 0x0001;
    internal const uint CUT         = 0x0004;
    internal const uint DROPHILITED = 0x0008;
    internal const uint GLOW        = 0x0010;
    internal const uint ACTIVATING  = 0x0020;
}
