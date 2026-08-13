using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;

namespace EasyWindowsApplication.WindowingModule.Backend;

internal delegate nint WNDPROC(nint hWnd, uint Msg, nint wParam, nint lParam);

internal static class Procedures
{
    private const int SW_SHOWNORMAL = 1;

    private static WNDPROC? _wndProcDelegate;
    private static MasterRouter? _router;

    internal static void SetRouter(MasterRouter router) => _router = router;

    internal static nint CreateMainWindow(
        MasterRouter router,
        string title,
        float width,
        float height)
    {
        nint hInstance = Win32.GetModuleHandleW(0);

        nint hIconLarge = Win32.LoadImageW(
            hInstance, (nint)1, IMAGE.ICON,
            Win32.GetSystemMetrics(11), Win32.GetSystemMetrics(12),
            LR.DEFAULTCOLOR);

        nint hIconSmall = Win32.LoadImageW(
            hInstance, (nint)1, IMAGE.ICON,
            Win32.GetSystemMetrics(49), Win32.GetSystemMetrics(50),
            LR.DEFAULTCOLOR);

        string className = $"EasyWinApp_{Guid.NewGuid():N}";
        nint classNamePtr = Marshal.StringToHGlobalUni(className);

        _wndProcDelegate = WndProcCallback;
        nint wndProcPtr = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate);

        var wndClass = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = CS.HREDRAW | CS.VREDRAW | CS.DBLCLKS,
            lpfnWndProc = wndProcPtr,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = hInstance,
            hIcon = hIconLarge,
            hCursor = 0,
            hbrBackground = 0,
            lpszMenuName = 0,
            lpszClassName = classNamePtr,
            hIconSm = hIconSmall
        };

        ushort atom = Win32.RegisterClassExW(ref wndClass);
        if (atom == 0)
        {
            Marshal.FreeHGlobal(classNamePtr);
            throw new InvalidOperationException($"RegisterClassExW failed: {Marshal.GetLastWin32Error()}");
        }

        nint titlePtr = Marshal.StringToHGlobalUni(title);
        nint hwnd = Win32.CreateWindowExW(
            0, classNamePtr, titlePtr,
            WS.OVERLAPPEDWINDOW | WS.CLIPCHILDREN,
            CW.USEDEFAULT, CW.USEDEFAULT, (int)width, (int)height,
            0, 0, hInstance, 0);

        Marshal.FreeHGlobal(classNamePtr);
        Marshal.FreeHGlobal(titlePtr);

        if (hwnd == 0)
            throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");

        _router = router;
        router.RegisterMainHwnd(hwnd);

        return hwnd;
    }

    internal static nint CreateAlternativeWindow(
        MasterRouter router,
        nint ownerHwnd,
        string title,
        int width,
        int height)
    {
        nint hInstance = Win32.GetModuleHandleW(0);

        string className = $"EasyWinAlt_{Guid.NewGuid():N}";
        nint classNamePtr = Marshal.StringToHGlobalUni(className);

        _wndProcDelegate = WndProcCallback;
        nint wndProcPtr = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate);

        var wndClass = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = CS.HREDRAW | CS.VREDRAW | CS.DBLCLKS,
            lpfnWndProc = wndProcPtr,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = hInstance,
            hIcon = 0,
            hCursor = 0,
            hbrBackground = 0,
            lpszMenuName = 0,
            lpszClassName = classNamePtr,
            hIconSm = 0
        };

        ushort atom = Win32.RegisterClassExW(ref wndClass);
        if (atom == 0)
        {
            Marshal.FreeHGlobal(classNamePtr);
            throw new InvalidOperationException($"RegisterClassExW failed: {Marshal.GetLastWin32Error()}");
        }

        nint titlePtr = Marshal.StringToHGlobalUni(title);
        nint hwnd = Win32.CreateWindowExW(
            0, classNamePtr, titlePtr,
            WS.POPUP | WS.CAPTION | WS.SYSMENU | WS.THICKFRAME | WS.MINIMIZEBOX | WS.MAXIMIZEBOX | WS.CLIPCHILDREN,
            CW.USEDEFAULT, CW.USEDEFAULT, width, height,
            ownerHwnd, 0, hInstance, 0);

        Marshal.FreeHGlobal(classNamePtr);
        Marshal.FreeHGlobal(titlePtr);

        if (hwnd == 0)
            throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");

        _router = router;

        return hwnd;
    }

    private static nint WndProcCallback(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (_router is not null)
            return _router.WndProc(hwnd, msg, wParam, lParam);

        return CoreModule.Backend.Win32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }
}
