using System.Runtime.InteropServices;

namespace EasyWindowsApplication.CoreModule.Backend;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
