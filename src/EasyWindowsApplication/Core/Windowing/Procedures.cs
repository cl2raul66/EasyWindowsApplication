using System.Runtime.InteropServices;
using EasyWindowsApplication.Core;

namespace EasyWindowsApplication.Core.Windowing;

internal static class Procedures
{
    private const int SW_SHOWNORMAL = 1;
    private const uint RT_GROUP_ICON = 14;
    private const int RT_MAINICON = 32512;
    private static int _iconResId = -1;

    internal static void SetRouter(MasterRouter router) { }

    private static int ResolveIconResourceId(nint hInstance)
    {
        if (_iconResId > 0)
            return _iconResId;

        int resolved = RT_MAINICON;
        for (int id = 1; id <= RT_MAINICON; id++)
        {
            if (Win32.FindResourceW(hInstance, (nint)id, (nint)RT_GROUP_ICON) != 0)
            {
                resolved = id;
                break;
            }
        }

        _iconResId = resolved;
        return resolved;
    }

    internal static unsafe nint CreateMainWindow(
        MasterRouter router,
        string title,
        float width,
        float height)
    {
        nint hInstance = Win32.GetModuleHandleW(0);
        int iconResId = ResolveIconResourceId(hInstance);

        nint hIconLarge = Win32.LoadImageW(
            hInstance, (nint)iconResId, IMAGE.ICON,
            0, 0,
            LR.DEFAULTSIZE | LR.CREATEDIBSECTION);

        nint hIconSmall = Win32.LoadImageW(
            hInstance, (nint)iconResId, IMAGE.ICON,
            0, 0,
            LR.DEFAULTSIZE | LR.CREATEDIBSECTION);

        string className = $"EasyWinApp_{Guid.NewGuid():N}";
        nint classNamePtr = Marshal.StringToHGlobalUni(className);

        nint wndProcPtr = (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&MasterRouter.WndProcTrampoline;

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

        Win32.SendMessageW(hwnd, WM.SETICON, 1, hIconLarge);
        Win32.SendMessageW(hwnd, WM.SETICON, 0, hIconSmall);

        HandleRegistry.RegisterRouter(hwnd, router);
        router.RegisterMainHwnd(hwnd);

        return hwnd;
    }

    internal static unsafe nint CreateAlternativeWindow(
        MasterRouter router,
        nint ownerHwnd,
        string title,
        int width,
        int height)
    {
        nint hInstance = Win32.GetModuleHandleW(0);

        string className = $"EasyWinAlt_{Guid.NewGuid():N}";
        nint classNamePtr = Marshal.StringToHGlobalUni(className);

        nint wndProcPtr = (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&MasterRouter.WndProcTrampoline;

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

        HandleRegistry.RegisterRouter(hwnd, router);

        return hwnd;
    }
}
