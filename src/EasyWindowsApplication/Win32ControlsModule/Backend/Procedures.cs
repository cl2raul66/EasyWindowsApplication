using System.Runtime.InteropServices;
using EasyWindowsApplication.Core;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal static class ControlProcedures
{
    private static nint _hInstance;
    private static nint _defaultFont;

    internal static void SetInstance(nint hInstance) => _hInstance = hInstance;

    internal static void InvalidateDefaultFont()
    {
        if (_defaultFont == 0) return;
        Win32Controls.DeleteObject(_defaultFont);
        _defaultFont = 0;
    }

    internal static nint GetDefaultFont()
    {
        if (_defaultFont != 0) return _defaultFont;

        var spec = UiDefaultsProvider.Current.DefaultFont;

        if (spec.IsSystemTheme)
        {
            var ncm = new NONCLIENTMETRICS
            {
                cbSize = (uint)Marshal.SizeOf<NONCLIENTMETRICS>()
            };
            if (Win32Controls.SystemParametersInfoW(SPI.GETNONCLIENTMETRICS, ncm.cbSize, ref ncm, 0))
            {
                _defaultFont = Win32Controls.CreateFontIndirectW(ref ncm.lfMessageFont);
                if (_defaultFont != 0) return _defaultFont;
            }
        }

        _defaultFont = CreateFontFromSpec(spec);
        return _defaultFont;
    }

    private static nint CreateFontFromSpec(FontSpec spec)
    {
        var lf = new LOGFONT
        {
            lfHeight = -(int)(spec.Size * GetDpiForSystemSafe() / 72f),
            lfWeight = spec.Weight == FontWeight.Bold ? 700 : 400,
            lfItalic = spec.Style == FontStyle.Italic ? (byte)1 : (byte)0,
            lfCharSet = 1, // DEFAULT_CHARSET
            lfQuality = 5, // CLEARTYPE_QUALITY
            lfFaceName = spec.Family ?? "Segoe UI"
        };
        var h = Win32Controls.CreateFontIndirectW(ref lf);
        if (h != 0) return h;
        // Ultimate fallback: GetStockObject DEFAULT_GUI_FONT (17)
        return Win32Controls.GetStockObject(17);
    }

    private static int GetDpiForSystemSafe()
    {
        try { return (int)Win32Controls.GetDpiForSystem(); } catch { }
        try
        {
            nint hdc = Win32Controls.GetDC(0);
            if (hdc != 0)
            {
                int dpi = Win32Controls.GetDeviceCaps(hdc, LOGPIXELS.LOGPIXELSY);
                Win32Controls.ReleaseDC(0, hdc);
                if (dpi > 0) return dpi;
            }
        }
        catch { }
        return 96;
    }

    private static int GetDpiForWindowSafe(nint hwnd)
    {
        try
        {
            uint dpi = Win32Controls.GetDpiForWindow(hwnd);
            if (dpi != 0) return (int)dpi;
        }
        catch { }
        return GetDpiForSystemSafe();
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
