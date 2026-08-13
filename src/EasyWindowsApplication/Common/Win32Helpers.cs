namespace EasyWindowsApplication.Common;

internal static class Win32Helpers
{
    internal static nint HIWORD(nint value) => (value >> 16) & 0xFFFF;
    internal static nint LOWORD(nint value) => value & 0xFFFF;
}
