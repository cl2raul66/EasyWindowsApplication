using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core;

internal static partial class Win32
{
    // ── System Color Indices (winuser.h) ──
    internal const int COLOR_WINDOW = 5;
    internal const int COLOR_WINDOWTEXT = 8;
    internal const int COLOR_BTNFACE = 15;
    internal const int COLOR_BTNTEXT = 18;

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

    // ── Window State (used by controls in Share) ──
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(nint lpModuleName);

    // ── GDI ──
    [LibraryImport("gdi32.dll")]
    internal static partial nint CreateSolidBrush(int color);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DeleteObject(nint hObject);

    [LibraryImport("gdi32.dll")]
    internal static partial nint GetStockObject(int fnObject);

    [LibraryImport("gdi32.dll")]
    internal static partial int SetBkMode(nint hdc, int mode);

    [LibraryImport("gdi32.dll")]
    internal static partial nint SelectObject(nint hdc, nint hgdiobj);

    [LibraryImport("gdi32.dll")]
    internal static partial int GetSysColor(int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial nint GetDC(nint hWnd);

    [LibraryImport("user32.dll")]
    internal static partial int ReleaseDC(nint hWnd, nint hDC);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(nint hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    internal static partial int FillRect(nint hDC, ref RECT lprc, nint hbr);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int DrawTextW(nint hdc, string lpchText, int nCount, ref RECT lprc, uint uFormat);

    [LibraryImport("user32.dll")]
    internal static partial nint SetClassLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    // ── GDI Text ──
    [LibraryImport("gdi32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetTextExtentPoint32W(nint hdc, string lpString, int c, out SIZE lpSize);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetCharABCWidthsW(nint hdc, uint uFirstChar, uint uLastChar, out ABC lpabc);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetCharWidth32W(nint hdc, uint uFirstChar, uint uLastChar, out int lpWidth);

    // ── UxTheme ──
    [LibraryImport("uxtheme.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int SetWindowTheme(nint hWnd, string pszSubAppName, string pszSubIdList);

    // ── Scroll ──
    [LibraryImport("user32.dll")]
    internal static partial int SetScrollInfo(nint hWnd, int nBar, ref SCROLLINFO lpsi, [MarshalAs(UnmanagedType.Bool)] bool fRedraw);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetScrollInfo(nint hWnd, int nBar, ref SCROLLINFO lpsi);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(nint hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ScreenToClient(nint hWnd, ref POINT lpPoint);

    // ── ComCtl32 ──
    [LibraryImport("comctl32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool InitCommonControlsEx(ref INITCOMMONCONTROLSEX lpInitCtrls);

    // ── DeferWindowPos (batching anti-flicker) ──
    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial nint BeginDeferWindowPos(int nNumWindows);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial nint DeferWindowPos(nint hWinPosInfo, nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EndDeferWindowPos(nint hWinPosInfo);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    internal static partial nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetMonitorInfoW(nint hMonitor, ref MONITORINFO lpmi);

    // SetWindowTextW con string (para Title)
    [LibraryImport("user32.dll", EntryPoint = "SetWindowTextW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowText(nint hWnd, string lpString);
}
