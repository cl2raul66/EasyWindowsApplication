using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class DateTimePicker : ControlBase<DateTimePicker>
{
    public DateTime Value
    {
        get
        {
            var st = new SYSTEMTIME();
            nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>());
            try
            {
                ControlProcedures.SendMessage(Hwnd, DTM.GETSYSTEMTIME, 0, ptr);
                st = Marshal.PtrToStructure<SYSTEMTIME>(ptr);
                return new DateTime(st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        set
        {
            var st = new SYSTEMTIME
            {
                wYear = (ushort)value.Year,
                wMonth = (ushort)value.Month,
                wDayOfWeek = (ushort)value.DayOfWeek,
                wDay = (ushort)value.Day,
                wHour = (ushort)value.Hour,
                wMinute = (ushort)value.Minute,
                wSecond = (ushort)value.Second,
                wMilliseconds = (ushort)value.Millisecond
            };
            nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>());
            try
            {
                Marshal.StructureToPtr(st, ptr, false);
                ControlProcedures.SendMessage(Hwnd, DTM.SETSYSTEMTIME, 0, ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    public string Format
    {
        set
        {
            nint textPtr = Marshal.StringToHGlobalUni(value);
            try
            {
                ControlProcedures.SendMessage(Hwnd, DTM.SETFORMAT, 0, textPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(textPtr);
            }
        }
    }

    public void SetRange(DateTime min, DateTime max)
    {
        var stMin = ToSystemTime(min);
        var stMax = ToSystemTime(max);

        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>() * 2);
        try
        {
            Marshal.StructureToPtr(stMin, ptr, false);
            Marshal.StructureToPtr(stMax, ptr + Marshal.SizeOf<SYSTEMTIME>(), false);
            ControlProcedures.SendMessage(Hwnd, DTM.SETRANGE, 0x0003 /* GDTR_MIN | GDTR_MAX */, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public (DateTime min, DateTime max) GetRange()
    {
        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEMTIME>() * 2);
        try
        {
            ControlProcedures.SendMessage(Hwnd, DTM.GETRANGE, 0, ptr);
            var stMin = Marshal.PtrToStructure<SYSTEMTIME>(ptr);
            var stMax = Marshal.PtrToStructure<SYSTEMTIME>(ptr + Marshal.SizeOf<SYSTEMTIME>());
            return (
                new DateTime(stMin.wYear, stMin.wMonth, stMin.wDay),
                new DateTime(stMax.wYear, stMax.wMonth, stMax.wDay)
            );
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public void CloseMonthCalendar()
    {
        ControlProcedures.SendMessage(Hwnd, DTM.CLOSEMONTHCAL, 0, 0);
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

[StructLayout(LayoutKind.Sequential)]
internal struct SYSTEMTIME
{
    internal ushort wYear;
    internal ushort wMonth;
    internal ushort wDayOfWeek;
    internal ushort wDay;
    internal ushort wHour;
    internal ushort wMinute;
    internal ushort wSecond;
    internal ushort wMilliseconds;
}
