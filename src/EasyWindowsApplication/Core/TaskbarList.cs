using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core;

internal static partial class TaskbarList
{
    [ComImport]
    [Guid("56FDF344-FD6D-11D0-958A-006097C9A090")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITaskbarList
    {
        void HrInit();
        void AddTab(nint hwnd);
        void DeleteTab(nint hwnd);
        void ActivateTab(nint hwnd);
        void SetActiveAlt(nint hwnd);
    }

    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(
        ref Guid rclsid, nint pUnkOuter, uint dwClsContext, ref Guid riid, out nint ppv);

    internal static bool SetVisible(nint hwnd, bool visible)
    {
        if (hwnd == 0) return false;
        try
        {
            var clsid = new Guid("56FDF344-FD6D-11D0-958A-006097C9A090");
            var iid = new Guid("56FDF342-FD6D-11D0-958A-006097C9A090");
            int hr = CoCreateInstance(ref clsid, 0, 1, ref iid, out nint ppv);
            if (hr != 0 || ppv == 0) return false;
            try
            {
                var list = (ITaskbarList)Marshal.GetObjectForIUnknown(ppv);
                try
                {
                    list.HrInit();
                    if (visible) list.AddTab(hwnd);
                    else list.DeleteTab(hwnd);
                    return true;
                }
                finally
                {
                    Marshal.ReleaseComObject(list);
                }
            }
            finally
            {
                Marshal.Release(ppv);
            }
        }
        catch
        {
            return false;
        }
    }
}
