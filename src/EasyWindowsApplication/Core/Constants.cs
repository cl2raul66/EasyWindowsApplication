namespace EasyWindowsApplication.Core;

    internal static class WS
    {
        internal const uint OVERLAPPED = 0x00000000;
        internal const uint POPUP = 0x80000000;
        internal const uint CHILD = 0x40000000;
        internal const uint VISIBLE = 0x10000000;
        internal const uint DISABLED = 0x08000000;
        internal const uint TABSTOP = 0x00010000;
        internal const uint BORDER = 0x00800000;
        internal const uint CAPTION = 0x00C00000;
        internal const uint SYSMENU = 0x00080000;
        internal const uint THICKFRAME = 0x00040000;
        internal const uint MINIMIZEBOX = 0x00020000;
        internal const uint MAXIMIZEBOX = 0x00010000;
        internal const uint VSCROLL = 0x00200000;
        internal const uint HSCROLL = 0x00100000;
        internal const uint CLIPCHILDREN = 0x02000000;
        internal const uint OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX;
    }
    
    internal static class WS_EX
    {
        internal const uint CLIENTEDGE = 0x00000200;
        internal const uint WINDOWEDGE = 0x00000100;
        internal const uint ACCEPTFILES = 0x00000010;
        internal const uint LAYOUTRTL = 0x00400000;
        internal const uint TOPMOST = 0x00000008;
    }
    
    internal static class CS
    {
        internal const uint HREDRAW = 0x0002;
        internal const uint VREDRAW = 0x0001;
        internal const uint DBLCLKS = 0x0008;
        internal const uint OWNDC = 0x0020;
    }
    
    internal static class CW
    {
        internal const int USEDEFAULT = unchecked((int)0x80000000);
    }
    
    internal static class IMAGE
    {
        internal const uint ICON = 1;
    }
    
    internal static class LR
    {
        internal const uint DEFAULTCOLOR = 0x00000000;
        internal const uint DEFAULTSIZE = 0x00000040;
        internal const uint CREATEDIBSECTION = 0x00002000;
    }
    
    internal static class WM
    {
        internal const uint NULL = 0x0000;
        internal const uint CREATE = 0x0001;
        internal const uint DESTROY = 0x0002;
        internal const uint MOVE = 0x0003;
        internal const uint SIZE = 0x0005;
        internal const uint ACTIVATE = 0x0006;
        internal const uint ERASEBKGND = 0x0014;
        internal const uint SETFOCUS = 0x0007;
        internal const uint KILLFOCUS = 0x0008;
        internal const uint ENABLE = 0x000A;
        internal const uint PAINT = 0x000F;
        internal const uint CLOSE = 0x0010;
        internal const uint QUIT = 0x0012;
        internal const uint SETTEXT = 0x000C;
        internal const uint GETTEXT = 0x000D;
        internal const uint SETICON = 0x0080;
        internal const uint GETTEXTLENGTH = 0x000E;
        internal const uint COMMAND = 0x0111;
        internal const uint SYSCOMMAND = 0x0112;
        internal const uint TIMER = 0x0113;
        internal const uint NOTIFY = 0x004E;
        internal const uint CTLCOLORBTN = 0x0135;
        internal const uint CTLCOLORDLG = 0x0136;
        internal const uint CTLCOLOREDIT = 0x0133;
        internal const uint CTLCOLORLISTBOX = 0x0134;
        internal const uint CTLCOLORMSGBOX = 0x0132;
        internal const uint CTLCOLORSCROLLBAR = 0x0137;
        internal const uint CTLCOLORSTATIC = 0x0138;
        internal const uint SETFONT = 0x0030;
        internal const uint GETFONT = 0x0031;
        internal const uint DRAWITEM = 0x002B;
        internal const uint HSCROLL = 0x0114;
        internal const uint VSCROLL = 0x0115;
        internal const uint MOUSEWHEEL = 0x020A;
        internal const uint ENTERSIZEMOVE = 0x0231;
        internal const uint EXITSIZEMOVE = 0x0232;
        internal const uint SETTINGCHANGE = 0x001A;
        internal const uint DPICHANGED = 0x02E0;
    }

    internal static class SPI
    {
        internal const uint GETNONCLIENTMETRICS = 0x0029;
        internal const uint SETNONCLIENTMETRICS = 0x002A;
    }

    internal static class WMSIZE
    {
        internal const int RESTORED = 0;
        internal const int MINIMIZED = 1;
        internal const int MAXIMIZED = 2;
        internal const int MAXSHOW = 3;
        internal const int MAXHIDE = 4;
    }

    internal static class WA
    {
        internal const int INACTIVE = 0;
        internal const int ACTIVE = 1;
        internal const int CLICKACTIVE = 2;
    }

    internal static class MONITOR
    {
        internal const uint DEFAULTTONULL = 0x00000000;
        internal const uint DEFAULTTOPRIMARY = 0x00000001;
        internal const uint DEFAULTTONEAREST = 0x00000002;
    }
    
    internal static class DT
    {
        internal const uint CALCRECT = 0x00000400;
        internal const uint SINGLELINE = 0x00000020;
        internal const uint NOPREFIX = 0x00000800;
    }
    
    internal static class SCROLLBAR
    {
        internal const int HORZ = 0;
        internal const int VERT = 1;
        internal const int LINEUP = 0;
        internal const int LINEDOWN = 1;
        internal const int PAGEUP = 2;
        internal const int PAGEDOWN = 3;
        internal const int THUMBTRACK = 5;
        internal const int TOP = 6;
        internal const int BOTTOM = 7;
        internal const int ENDSCROLL = 8;
    }

    internal static class SIF
    {
        internal const uint RANGE = 0x0001;
        internal const uint PAGE = 0x0002;
        internal const uint POS = 0x0004;
        internal const uint ALL = RANGE | PAGE | POS;
    }

    internal static class BN
    {
        internal const uint CLICKED = 0;
        internal const uint DOUBLECLICKED = 5;
    }
    
    internal static class EN
    {
        internal const uint CHANGE = 0x0300;
        internal const uint UPDATE = 0x0104;
        internal const uint MAXTEXT = 0x0105;
    }
    
    internal static class SW
    {
        internal const int HIDE = 0;
        internal const int SHOWNORMAL = 1;
        internal const int SHOWMINIMIZED = 2;
        internal const int SHOWMAXIMIZED = 3;
        internal const int SHOWNOACTIVATE = 4;
        internal const int SHOW = 5;
        internal const int MINIMIZE = 6;
        internal const int SHOWMINNOACTIVE = 7;
        internal const int SHOWNORMAL2 = 8;
        internal const int RESTORE = 9;
        internal const int SHOWDEFAULT = 10;
        internal const int MAXIMIZE = 3;
    }
    
    internal static class GWL
    {
        internal const int STYLE = -16;
        internal const int EXSTYLE = -20;
        internal const int ID = -12;
    }

    internal static class ICC
    {
        internal const uint LISTVIEW_CLASSES = 0x00000001;
        internal const uint TREEVIEW_CLASSES = 0x00000002;
        internal const uint BAR_CLASSES = 0x00000004;
        internal const uint TAB_CLASSES = 0x00000008;
        internal const uint UPDOWN_CLASS = 0x00000010;
        internal const uint PROGRESS_CLASS = 0x00000020;
        internal const uint HOTKEY_CLASS = 0x00000040;
        internal const uint ANIMATE_CLASS = 0x00000080;
        internal const uint WIN95_CLASSES = 0x000000FF;
        internal const uint DATE_CLASSES = 0x00000100;
        internal const uint USEREX_CLASSES = 0x00000200;
        internal const uint COOL_CLASSES = 0x00000400;
        internal const uint INTERNET_CLASSES = 0x00000800;
        internal const uint PAGESCROLLER_CLASS = 0x00001000;
        internal const uint NATIVEFNTCTL_CLASS = 0x00002000;
        internal const uint STANDARD_CLASSES = 0x00004000;
        internal const uint LINK_CLASS = 0x00008000;
    }

    internal static class SWP
    {
        internal const uint NOSIZE = 0x0001;
        internal const uint NOMOVE = 0x0002;
        internal const uint NOZORDER = 0x0004;
        internal const uint NOREDRAW = 0x0008;
        internal const uint NOACTIVATE = 0x0010;
        internal const uint FRAMECHANGED = 0x0020;
        internal const uint SHOWWINDOW = 0x0040;
        internal const uint HIDEWINDOW = 0x0080;
        internal const uint NOCOPYBITS = 0x0100;
        internal const uint NOOWNERZORDER = 0x0200;
        internal const uint NOSENDCHANGING = 0x0400;
    }
