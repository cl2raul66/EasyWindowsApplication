namespace EasyWindowsApplication.Share;

public static class WM
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
}

public static class BN
{
    public const uint CLICKED = 0;
    public const uint DOUBLECLICKED = 5;
}
