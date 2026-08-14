using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal static class ControlProcedures
{
    private static nint _hInstance;
    private static nint _defaultFont;

    internal static void SetInstance(nint hInstance) => _hInstance = hInstance;

    internal static nint GetDefaultFont()
    {
        if (_defaultFont == 0)
            _defaultFont = Win32.GetStockObject(17); // DEFAULT_GUI_FONT
        return _defaultFont;
    }

    internal static nint CreateControl(
        string windowClass,
        nint parentHwnd,
        uint style,
        uint exStyle,
        string text,
        int x, int y, int w, int h,
        nint hMenu)
    {
        nint classPtr = Marshal.StringToHGlobalUni(windowClass);
        nint textPtr = Marshal.StringToHGlobalUni(text);

        nint hwnd = Win32Controls.CreateWindowExW(
            exStyle, classPtr, textPtr,
            style,
            x, y, w, h,
            parentHwnd, hMenu, _hInstance, 0);

        Marshal.FreeHGlobal(classPtr);
        Marshal.FreeHGlobal(textPtr);

        if (hwnd != 0)
            Win32Controls.SendMessageW(hwnd, WM.SETFONT, GetDefaultFont(), 1);

        return hwnd;
    }

    internal static string GetWindowText(nint hwnd)
    {
        int length = Win32Controls.GetWindowTextLengthW(hwnd);
        if (length == 0) return string.Empty;

        nint buffer = Marshal.AllocHGlobal((length + 1) * 2);
        try
        {
            _ = Win32Controls.GetWindowTextW(hwnd, buffer, length + 1);
            return Marshal.PtrToStringUni(buffer, length) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    internal static void SetWindowText(nint hwnd, string text)
    {
        nint textPtr = Marshal.StringToHGlobalUni(text);
        try
        {
            Win32Controls.SetWindowTextW(hwnd, textPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    internal static nint SendMessage(nint hwnd, uint msg, nint wParam, nint lParam)
        => Win32Controls.SendMessageW(hwnd, msg, wParam, lParam);
}
