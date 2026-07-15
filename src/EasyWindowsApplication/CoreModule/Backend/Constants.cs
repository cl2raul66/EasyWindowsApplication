namespace EasyWindowsApplication.CoreModule.Backend;

internal static class WS
{
    public const uint OVERLAPPED = 0x00000000;
    public const uint POPUP = 0x80000000;
    public const uint CHILD = 0x40000000;
    public const uint VISIBLE = 0x10000000;
    public const uint DISABLED = 0x08000000;
    public const uint TABSTOP = 0x00010000;
    public const uint BORDER = 0x00800000;
    public const uint CAPTION = 0x00C00000;
    public const uint SYSMENU = 0x00080000;
    public const uint THICKFRAME = 0x00040000;
    public const uint MINIMIZEBOX = 0x00020000;
    public const uint MAXIMIZEBOX = 0x00010000;
    public const uint VSCROLL = 0x00200000;
    public const uint HSCROLL = 0x00100000;
    public const uint OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX;
}

internal static class WS_EX
{
    public const uint CLIENTEDGE = 0x00000200;
    public const uint WINDOWEDGE = 0x00000100;
    public const uint ACCEPTFILES = 0x00000010;
    public const uint LAYOUTRTL = 0x00400000;
    public const uint TOPMOST = 0x00000008;
}

internal static class CS
{
    public const uint HREDRAW = 0x0002;
    public const uint VREDRAW = 0x0001;
    public const uint DBLCLKS = 0x0008;
    public const uint OWNDC = 0x0020;
}

internal static class WM
{
    public const uint NULL = 0x0000;
    public const uint CREATE = 0x0001;
    public const uint DESTROY = 0x0002;
    public const uint MOVE = 0x0003;
    public const uint SIZE = 0x0005;
    public const uint ACTIVATE = 0x0006;
    public const uint SETFOCUS = 0x0007;
    public const uint KILLFOCUS = 0x0008;
    public const uint ENABLE = 0x000A;
    public const uint PAINT = 0x000F;
    public const uint CLOSE = 0x0010;
    public const uint QUIT = 0x0012;
    public const uint SETTEXT = 0x000C;
    public const uint GETTEXT = 0x000D;
    public const uint GETTEXTLENGTH = 0x000E;
    public const uint COMMAND = 0x0111;
    public const uint SYSCOMMAND = 0x0112;
    public const uint TIMER = 0x0113;
    public const uint NOTIFY = 0x004E;
    public const uint CTLCOLORBTN = 0x0135;
    public const uint CTLCOLORDLG = 0x0136;
    public const uint CTLCOLOREDIT = 0x0133;
    public const uint CTLCOLORLISTBOX = 0x0134;
    public const uint CTLCOLORMSGBOX = 0x0132;
    public const uint CTLCOLORSCROLLBAR = 0x0137;
    public const uint CTLCOLORSTATIC = 0x0138;
}

internal static class BN
{
    public const uint CLICKED = 0;
    public const uint DOUBLECLICKED = 5;
}

internal static class EN
{
    public const uint CHANGE = 0x0300;
    public const uint UPDATE = 0x0104;
    public const uint MAXTEXT = 0x0105;
}

internal static class CW
{
    public const int USEDEFAULT = unchecked((int)0x80000000);
}

internal static class GWL
{
    public const int STYLE = -16;
    public const int EXSTYLE = -20;
    public const int ID = -12;
}

internal static class IMAGE
{
    public const uint ICON = 1;
}

internal static class LR
{
    public const uint DEFAULTCOLOR = 0x0040;
}
