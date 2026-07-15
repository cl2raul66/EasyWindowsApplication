using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal static partial class Win32Controls
{
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindowExW(
        uint dwExStyle, nint lpClassName, nint lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight,
        nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetWindowTextW(nint hWnd, nint lpString, int nMaxCount);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowTextW(nint hWnd, nint lpString);

    [LibraryImport("user32.dll")]
    internal static partial int GetWindowTextLengthW(nint hWnd);

    [LibraryImport("kernel32.dll")]
    internal static partial nint GetModuleHandleW(nint lpModuleName);
}
