using System.Runtime.InteropServices;

namespace EasyWindowsApplication.CoreModule.Backend;

internal static partial class Win32
{
    // ── Window Class ──
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial ushort RegisterClassExW(ref WNDCLASSEXW lpWndClass);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterClassW(nint lpClassName, nint hInstance);

    // ── Window Creation ──
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindowExW(
        uint dwExStyle, nint lpClassName, nint lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight,
        nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    // ── Window State ──
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hWnd);

    // ── Message Loop ──
    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial int GetMessageW(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(ref MSG lpMsg);

    [LibraryImport("user32.dll")]
    internal static partial nint DispatchMessageW(ref MSG lpMsg);

    [LibraryImport("user32.dll")]
    internal static partial nint DefWindowProcW(nint hWnd, uint Msg, nint wParam, nint lParam);

    // ── Messages ──
    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    internal static partial void PostQuitMessage(int nExitCode);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint LoadImageW(nint hInst, nint name, uint type, int cx, int cy, uint fuLoad);

    // ── Text ──
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetWindowTextW(nint hWnd, nint lpString, int nMaxCount);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowTextW(nint hWnd, nint lpString);

    [LibraryImport("user32.dll")]
    internal static partial int GetWindowTextLengthW(nint hWnd);

    // ── Window Properties ──
    [LibraryImport("user32.dll")]
    internal static partial int GetWindowLongW(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowLongW(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport("user32.dll")]
    internal static partial nint GetWindowLongPtrW(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    // ── Instance ──
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(nint lpModuleName);

    // ── Clipboard ──
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool OpenClipboard(nint hWndNewOwner);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseClipboard();

    [LibraryImport("user32.dll")]
    internal static partial nint GetClipboardData(uint uFormat);

    [LibraryImport("user32.dll")]
    internal static partial nint SetClipboardData(uint uFormat, nint hMem);

    // ── Memory ──
    [LibraryImport("kernel32.dll")]
    internal static partial nint GlobalAlloc(uint uFlags, nuint dwBytes);

    [LibraryImport("kernel32.dll")]
    internal static partial nint GlobalLock(nint hMem);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GlobalUnlock(nint hMem);

    [LibraryImport("kernel32.dll")]
    internal static partial nuint GlobalSize(nint hMem);

    // ── Atoms ──
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial ushort GlobalAddAtomW(nint lpString);

    // ── Rectangles ──
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(nint hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int nIndex);
}
