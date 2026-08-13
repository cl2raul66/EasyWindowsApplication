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

// ── BM (Button Messages) ──
internal static class BM
{
    internal const uint CLICK       = 0x00F2;
    internal const uint GETCHECK    = 0x00F0;
    internal const uint SETCHECK    = 0x00F1;
    internal const uint GETSTATE    = 0x00F2;
    internal const uint SETSTATE    = 0x00F3;
    internal const uint SETSTYLE    = 0x00F4;
    internal const uint GETIMAGE    = 0x00F6;
    internal const uint SETIMAGE    = 0x00F7;
}

// ── EM (Edit Messages) ──
internal static class EM
{
    internal const uint GETSEL          = 0x00B0;
    internal const uint SETSEL          = 0x00B1;
    internal const uint GETRECT         = 0x00B2;
    internal const uint SETRECT         = 0x00B3;
    internal const uint SETMODIFY       = 0x00B9;
    internal const uint GETMODIFY       = 0x00B8;
    internal const uint GETSCROLLPOS    = 0x00DD;
    internal const uint SETSCROLLPOS    = 0x00DE;
    internal const uint SCROLLCARET     = 0x00B7;
    internal const uint GETLINECOUNT    = 0x00BA;
    internal const uint SETLINECOUNT    = 0x00C8;
    internal const uint GETLINE         = 0x00C4;
    internal const uint LINELENGTH      = 0x00C1;
    internal const uint LINEFROMCHAR    = 0x00C9;
    internal const uint REPLACESEL      = 0x00C2;
    internal const uint GETPASSWORDCHAR = 0x00D7;
    internal const uint SETPASSWORDCHAR = 0x00CC;
    internal const uint SETREADONLY     = 0x00CF;
    internal const uint GETMARGINS      = 0x00D4;
    internal const uint SETMARGINS      = 0x00D3;
    internal const uint GETHANDLE       = 0x00BD;
    internal const uint SETHANDLE       = 0x00BC;
    internal const uint UNDO            = 0x00C7;
    internal const uint CANUNDO         = 0x00C6;
    internal const uint EMPTYUNDOBUFFER = 0x00CD;
    internal const uint GETIMESTATUS    = 0x0126;
    internal const uint SETIMESTATUS    = 0x0127;
    internal const uint GETFIRSTVISIBLELINE = 0x00CE;
    internal const uint SETLIMITTEXT    = 0x00C5;
    internal const uint GETLIMITTEXT    = 0x00D0;
}

// ── STM (Static Messages) ──
internal static class STM
{
    internal const uint SETICON  = 0x0170;
    internal const uint GETICON  = 0x0171;
    internal const uint SETIMAGE = 0x0172;
    internal const uint GETIMAGE = 0x0173;
}

// ── LB (ListBox Messages) ──
internal static class LB
{
    internal const uint ADDSTRING        = 0x0180;
    internal const uint INSERTSTRING     = 0x0181;
    internal const uint DELETESTRING     = 0x0182;
    internal const uint DIR              = 0x018D;
    internal const uint GETCOUNT         = 0x018B;
    internal const uint RESETCONTENT     = 0x0184;
    internal const uint SETSEL           = 0x0185;
    internal const uint GETSEL           = 0x0187;
    internal const uint SETCURSEL        = 0x0186;
    internal const uint GETCURSEL        = 0x0188;
    internal const uint GETTEXT          = 0x0189;
    internal const uint GETTEXTLEN       = 0x018A;
    internal const uint ADDFILE          = 0x0196;
    internal const uint SETTOPINDEX      = 0x0197;
    internal const uint GETTOPINDEX      = 0x018E;
    internal const uint FINDSTRING       = 0x018F;
    internal const uint GETSELCOUNT      = 0x0190;
    internal const uint GETSELITEMS      = 0x0191;
    internal const uint SETTABSTOPS      = 0x0192;
    internal const uint GETHORIZONTALEXTENT = 0x0193;
    internal const uint SETHORIZONTALEXTENT = 0x0194;
    internal const uint SETCOLUMNWIDTH   = 0x0195;
    internal const uint ADDSTRINGA       = 0x0180;
    internal const uint ADDSTRINGW       = 0x0180;
    internal const uint INSERTSTRINGA    = 0x0181;
    internal const uint INSERTSTRINGW    = 0x0181;
    internal const uint GETTEXTA         = 0x0189;
    internal const uint GETTEXTW         = 0x0189;
    internal const uint FINDSTRINGEXACT  = 0x01A2;
    internal const uint SETLOCALE        = 0x01A5;
    internal const uint GETLOCALE        = 0x01A6;
    internal const uint INITSTORAGE      = 0x0198;
    internal const uint ITEMFROMPOINT    = 0x01A9;
    internal const uint GETLISTBOXINFO   = 0x01B2;
}

// ── CB (ComboBox Messages) ──
internal static class CB
{
    internal const uint ADDSTRING         = 0x0143;
    internal const uint DELETESTRING      = 0x0144;
    internal const uint DIR              = 0x0145;
    internal const uint GETCOUNT         = 0x0146;
    internal const uint GETCURSEL        = 0x0147;
    internal const uint GETLBTEXT        = 0x0148;
    internal const uint GETLBTEXTLEN     = 0x0149;
    internal const uint INSERTSTRING     = 0x014A;
    internal const uint RESETCONTENT     = 0x014B;
    internal const uint FINDSTRING       = 0x014C;
    internal const uint SELECTSTRING     = 0x014D;
    internal const uint SETCURSEL        = 0x014E;
    internal const uint SHOWDROPDOWN     = 0x014F;
    internal const uint GETITEMDATA      = 0x0150;
    internal const uint SETITEMDATA      = 0x0151;
    internal const uint GETDROPPEDCONTROLRECT = 0x0152;
    internal const uint SETITEMHEIGHT    = 0x0153;
    internal const uint GETITEMHEIGHT    = 0x0154;
    internal const uint SETEXTENDEDUI    = 0x0155;
    internal const uint GETEXTENDEDUI    = 0x0156;
    internal const uint GETDROPPEDSTATE  = 0x0157;
    internal const uint FINDSTRINGEXACT  = 0x0158;
    internal const uint SETLOCALE        = 0x0159;
    internal const uint GETLOCALE        = 0x015A;
    internal const uint GETTOPINDEX      = 0x015B;
    internal const uint SETTOPINDEX      = 0x015C;
    internal const uint GETHORIZONTALEXTENT = 0x015D;
    internal const uint SETHORIZONTALEXTENT = 0x015E;
    internal const uint GETDROPPEDWIDTH  = 0x015F;
    internal const uint SETDROPPEDWIDTH  = 0x0160;
    internal const uint INITSTORAGE      = 0x0161;
    internal const uint GETMINVISIBLE    = 0x0162;
    internal const uint SETMINVISIBLE    = 0x0163;
    internal const uint GETCOMBOBOXINFO  = 0x0164;
    internal const uint LIMITTEXT        = 0x0141;
    internal const uint GETEDITSEL       = 0x0140;
    internal const uint SETEDITSEL       = 0x0142;
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
    internal const uint SETSTEP     = 0x0404;
    internal const uint STEPIT      = 0x0405;
    internal const uint SETRANGE32  = 0x0406;
    internal const uint GETRANGE    = 0x0407;
    internal const uint GETPOS      = 0x0408;
    internal const uint SETBARCOLOR = 0x0409;
    internal const uint SETBKCOLOR  = 0x040A;
    internal const uint SETMARQUEE  = 0x040B;
    internal const uint GETSTEP     = 0x040C;
    internal const uint GETBKCOLOR  = 0x040D;
    internal const uint GETBARCOLOR = 0x040E;
    internal const uint SETSTATE    = 0x0410;
    internal const uint GETSTATE    = 0x0411;
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
    internal const uint SETFORMAT     = 0x1006;
    internal const uint GETMCCOLOR    = 0x1007;
    internal const uint SETMCCOLOR    = 0x1008;
    internal const uint GETMCFONT     = 0x1009;
    internal const uint SETMCFONT     = 0x100A;
    internal const uint SETMCSTYLE    = 0x100B;
    internal const uint GETMCSTYLE    = 0x100C;
    internal const uint CLOSEMONTHCAL = 0x100D;
    internal const uint GETDATETIMEPICKERINFO = 0x100E;
    internal const uint GETMONTHCAL   = 0x100F;
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
    internal const uint GETMONTHCOLOR   = FIRST + 11;
    internal const uint SETMONTHCOLOR   = FIRST + 12;
    internal const uint GETCOLOR        = FIRST + 13;
    internal const uint SETCOLOR        = FIRST + 14;
    internal const uint GETRECT         = FIRST + 15;
    internal const uint SETRECT         = FIRST + 16;
    internal const uint GETMINREQRECT   = FIRST + 17;
    internal const uint SETTODAY        = FIRST + 19;
    internal const uint GETTODAY        = FIRST + 20;
    internal const uint HITTEST         = FIRST + 21;
    internal const uint SETFIRSTDAYOFWEEK = FIRST + 22;
    internal const uint GETFIRSTDAYOFWEEK = FIRST + 23;
    internal const uint GETRANGEMIN     = FIRST + 24;
    internal const uint GETRANGEMAX     = FIRST + 25;
    internal const uint GETMONTHDELTA   = FIRST + 26;
    internal const uint SETMONTHDELTA   = FIRST + 27;
    internal const uint GETNEXTMONTHDELTA = FIRST + 28;
    internal const uint GETUNICODEFORMAT = FIRST + 29;
    internal const uint SETUNICODEFORMAT = FIRST + 30;
}

// ── HKM (HotKey Messages) ──
internal static class HKM
{
    internal const uint SETHOTKEY  = 0x0401;
    internal const uint GETHOTKEY  = 0x0402;
    internal const uint RULES      = 0x0403;
}

// ── TB (Toolbar Messages) ──
internal static class TB
{
    internal const uint FIRST           = 0x0400;
    internal const uint ENABLEBUTTON    = FIRST + 1;
    internal const uint CHECKBUTTON     = FIRST + 2;
    internal const uint PRESSBUTTON     = FIRST + 3;
    internal const uint HIGHLIGHTITEM   = FIRST + 13;
    internal const uint ISBUTTONCHECKED = FIRST + 10;
    internal const uint ISBUTTONDISABLED = FIRST + 9;
    internal const uint ISBUTTONDOWN    = FIRST + 11;
    internal const uint ISBUTTONHIDDEN  = FIRST + 12;
    internal const uint ISBUTTONPRESSED = FIRST + 8;
    internal const uint ADDBITMAP       = FIRST + 19;
    internal const uint ADDBUTTONS      = FIRST + 20;
    internal const uint ADDSTRING       = FIRST + 28;
    internal const uint AUTOSIZE        = FIRST + 25;
    internal const uint BUTTONCOUNT     = FIRST + 24;
    internal const uint BUTTONSTRUCTSIZE = FIRST + 30;
    internal const uint CHANGEBITMAP    = FIRST + 43;
    internal const uint COMMANDTOINDEX  = FIRST + 25;
    internal const uint CUSTOMIZE       = FIRST + 27;
    internal const uint DELETEBUTTON    = FIRST + 22;
    internal const uint GETANCHORHIGHLIGHT = FIRST + 52;
    internal const uint GETBITMAP       = FIRST + 44;
    internal const uint GETBITMAPFLAGS  = FIRST + 41;
    internal const uint GETBUTTON       = FIRST + 23;
    internal const uint GETBUTTONTEXTA  = FIRST + 45;
    internal const uint GETBUTTONTEXTW  = FIRST + 75;
    internal const uint GETDISABLEDIMAGELIST = FIRST + 55;
    internal const uint GETEXTENDEDSTYLE = FIRST + 85;
    internal const uint GETHOTIMAGELIST = FIRST + 57;
    internal const uint GETHOTITEM      = FIRST + 61;
    internal const uint GETIMAGELIST    = FIRST + 49;
    internal const uint GETITEMRECT     = FIRST + 29;
    internal const uint GETMAXSIZE      = FIRST + 63;
    internal const uint GETPRESSEDIMAGELIST = FIRST + 66;
    internal const uint GETROWS         = FIRST + 40;
    internal const uint GETSTATE        = FIRST + 33;
    internal const uint GETSTYLE        = FIRST + 41;
    internal const uint GETTOOLTIPS     = FIRST + 51;
    internal const uint GETUNICODEFORMAT = FIRST + 86;
    internal const uint MARKBUTTON      = FIRST + 6;
    internal const uint MOVEBUTTON      = FIRST + 82;
    internal const uint REPLACEBITMAP   = FIRST + 46;
    internal const uint SAVERESTOREA    = FIRST + 48;
    internal const uint SAVERESTOREW    = FIRST + 78;
    internal const uint SETANCHORHIGHLIGHT = FIRST + 53;
    internal const uint SETBITMAPSIZE   = FIRST + 32;
    internal const uint SETBUTTONWIDTH  = FIRST + 59;
    internal const uint SETBUTTONSIZE   = FIRST + 31;
    internal const uint SETDISABLEDIMAGELIST = FIRST + 54;
    internal const uint SETEXTENDEDSTYLE = FIRST + 84;
    internal const uint SETHOTIMAGELIST = FIRST + 56;
    internal const uint SETHOTITEM      = FIRST + 60;
    internal const uint SETIMAGELIST    = FIRST + 48;
    internal const uint SETINDENT       = FIRST + 47;
    internal const uint SETMAXTEXTROWS  = FIRST + 60;
    internal const uint SETMARGINS      = FIRST + 58;
    internal const uint SETPARENT       = FIRST + 55;
    internal const uint SETROWS         = FIRST + 39;
    internal const uint SETSTATE        = FIRST + 34;
    internal const uint SETSTYLE        = FIRST + 42;
    internal const uint SETTOOLTIPS     = FIRST + 50;
    internal const uint SETUNICODEFORMAT = FIRST + 85;
}

// ── SBM (ScrollBar Messages) ──
internal static class SBM
{
    internal const uint SETPOS           = 0x00E9;
    internal const uint GETPOS           = 0x00EA;
    internal const uint SETRANGE         = 0x00E7;
    internal const uint GETRANGE         = 0x00E8;
    internal const uint ENABLE_ARROWS    = 0x00E4;
    internal const uint SETSCROLLINFO    = 0x00E9;
    internal const uint GETSCROLLINFO    = 0x00EA;
    internal const uint GETSCROLLBARINFO = 0x00EB;
    internal const uint SETRANGEREDRAW   = 0x00E6;
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
    internal const uint SETBUDDY      = FIRST + 33;
    internal const uint GETBUDDY      = FIRST + 34;
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
    internal const uint GETPARTS     = 0x0406;
    internal const uint GETTIPTEXTA  = 0x0412;
    internal const uint GETTIPTEXTW  = 0x0413;
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
    internal const uint CREATEDRAGIMAGE = FIRST + 19;
    internal const uint SORTCHILDREN   = FIRST + 20;
    internal const uint ENSUREVISIBLE  = FIRST + 22;
    internal const uint GETCOUNT       = FIRST + 18;
    internal const uint SETBKCOLOR     = FIRST + 29;
    internal const uint SETTEXTCOLOR   = FIRST + 30;
    internal const uint GETBKCOLOR     = FIRST + 31;
    internal const uint GETTEXTCOLOR   = FIRST + 32;
    internal const uint GETISEARCHSTRINGA = FIRST + 23;
    internal const uint GETISEARCHSTRINGW = FIRST + 64;
    internal const uint SETITEMHEIGHT   = FIRST + 27;
    internal const uint GETITEMHEIGHT   = FIRST + 28;
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
    internal const uint GETITEMCOUNT = FIRST + 4;
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
