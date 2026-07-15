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
