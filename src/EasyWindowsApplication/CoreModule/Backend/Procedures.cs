namespace EasyWindowsApplication.CoreModule.Backend;

internal static class Procedures
{
    internal static void RunMessageLoop()
    {
        while (Win32.GetMessageW(out MSG msg, 0, 0, 0) > 0)
        {
            Win32.TranslateMessage(ref msg);
            Win32.DispatchMessageW(ref msg);
        }
    }
}
