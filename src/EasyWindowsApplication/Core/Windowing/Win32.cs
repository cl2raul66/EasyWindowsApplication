using System.Runtime.InteropServices;
using EasyWindowsApplication.Core;

namespace EasyWindowsApplication.Core.Windowing;

internal static partial class Win32
{
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial ushort RegisterClassExW(ref WNDCLASSEXW lpWndClass);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterClassW(nint lpClassName, nint hInstance);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindowExW(
        uint dwExStyle, nint lpClassName, nint lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight,
        nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hWnd);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint LoadImageW(nint hInst, nint name, uint type, int cx, int cy, uint fuLoad);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(nint hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int nIndex);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(nint lpModuleName);
}
