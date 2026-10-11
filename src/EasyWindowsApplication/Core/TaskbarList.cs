using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core;

internal static partial class TaskbarList
{
    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(
        ref Guid rclsid, nint pUnkOuter, uint dwClsContext, ref Guid riid, out nint ppv);

    // ITaskbarList vtable: 0 QueryInterface, 1 AddRef, 2 Release,
    // 3 HrInit, 4 AddTab, 5 DeleteTab, 6 ActivateTab, 7 SetActiveAlt.
    // Llamada manual (sin ComImport: el dispatch clásico lanza
    // NotSupportedException en .NET moderno). AOT-safe.
    internal static unsafe bool SetVisible(nint hwnd, bool visible)
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
                nint* vtable = *(nint**)ppv;
                var hrInit = (delegate* unmanaged[Stdcall]<nint, int>)vtable[3];
                var addTab = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[4];
                var deleteTab = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[5];
                if (hrInit(ppv) != 0) return false;
                int result = visible ? addTab(ppv, hwnd) : deleteTab(ppv, hwnd);
                return result == 0;
            }
            finally
            {
                var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)ppv)[2]);
                release(ppv);
            }
        }
        catch
        {
            return false;
        }
    }
}
