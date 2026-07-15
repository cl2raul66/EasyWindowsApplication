using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class MonthCalendar : ControlBase<MonthCalendar>
{
    public DateTime SelectionStart
    {
        get
        {
            var st = new SYSTEMTIME();
            nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>() * 2);
            try
            {
                ControlProcedures.SendMessage(Hwnd, MCM.GETSELRANGE, 0, ptr);
                st = Marshal.PtrToStructure<SYSTEMTIME>(ptr);
                return new DateTime(st.wYear, st.wMonth, st.wDay);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    public DateTime SelectionEnd
    {
        get
        {
            var st = new SYSTEMTIME();
            nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>() * 2);
            try
            {
                ControlProcedures.SendMessage(Hwnd, MCM.GETSELRANGE, 0, ptr);
                nint secondPtr = ptr + Marshal.SizeOf<SYSTEMTIME>();
                st = Marshal.PtrToStructure<SYSTEMTIME>(secondPtr);
                return new DateTime(st.wYear, st.wMonth, st.wDay);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    public void SetSelectionRange(DateTime start, DateTime end)
    {
        var stStart = ToSystemTime(start);
        var stEnd = ToSystemTime(end);

        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>() * 2);
        try
        {
            Marshal.StructureToPtr(stStart, ptr, false);
            Marshal.StructureToPtr(stEnd, ptr + Marshal.SizeOf<SYSTEMTIME>(), false);
            ControlProcedures.SendMessage(Hwnd, MCM.SETSELRANGE, 0, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static SYSTEMTIME ToSystemTime(DateTime dt) => new()
    {
        wYear = (ushort)dt.Year,
        wMonth = (ushort)dt.Month,
        wDayOfWeek = (ushort)dt.DayOfWeek,
        wDay = (ushort)dt.Day,
        wHour = (ushort)dt.Hour,
        wMinute = (ushort)dt.Minute,
        wSecond = (ushort)dt.Second,
        wMilliseconds = (ushort)dt.Millisecond
    };
}
