using System.Runtime.InteropServices;
using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class UpDown : ControlBase<UpDown>
{
    public int Position
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, UDM.GETPOS, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETPOS, 0, (nint)value);
    }

    public int Position32
    {
        get => (int)ControlProcedures.SendMessage(Hwnd, UDM.GETPOS32, 0, 0);
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETPOS32, 0, (nint)value);
    }

    public (int min, int max) Range
    {
        set
        {
            nint packed = ((nint)value.max << 16) | (ushort)value.min;
            ControlProcedures.SendMessage(Hwnd, UDM.SETRANGE, 0, packed);
        }
    }

    public nint Buddy
    {
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETBUDDY, value, 0);
    }

    public int Base
    {
        set => ControlProcedures.SendMessage(Hwnd, UDM.SETBASE, (nint)value, 0);
    }

    public int GetBase()
    {
        return (int)ControlProcedures.SendMessage(Hwnd, UDM.GETBASE, 0, 0);
    }

    public nint GetBuddy()
    {
        return ControlProcedures.SendMessage(Hwnd, UDM.GETBUDDY, 0, 0);
    }

    public (int min, int max) GetRange()
    {
        nint minPtr = Marshal.AllocHGlobal(4);
        nint maxPtr = Marshal.AllocHGlobal(4);
        try
        {
            ControlProcedures.SendMessage(Hwnd, UDM.GETRANGE, maxPtr, minPtr);
            int min = Marshal.ReadInt32(minPtr);
            int max = Marshal.ReadInt32(maxPtr);
            return (min, max);
        }
        finally
        {
            Marshal.FreeHGlobal(minPtr);
            Marshal.FreeHGlobal(maxPtr);
        }
    }

    public (int accel, int sec) GetAccel()
    {
        var accel = new UDACCEL();
        nint accelPtr = Marshal.AllocHGlobal(Marshal.SizeOf<UDACCEL>());
        try
        {
            Marshal.StructureToPtr(accel, accelPtr, false);
            ControlProcedures.SendMessage(Hwnd, UDM.GETACCEL, 1, accelPtr);
            accel = Marshal.PtrToStructure<UDACCEL>(accelPtr);
            return (accel.nSec, accel.nInc);
        }
        finally
        {
            Marshal.FreeHGlobal(accelPtr);
        }
    }

    public void SetAccel(int[] accels)
    {
        var accelArray = new UDACCEL[accels.Length];
        for (int i = 0; i < accels.Length; i++)
        {
            accelArray[i] = new UDACCEL { nSec = i + 1, nInc = accels[i] };
        }

        int size = Marshal.SizeOf<UDACCEL>() * accels.Length;
        nint ptr = Marshal.AllocHGlobal(size);
        try
        {
            for (int i = 0; i < accels.Length; i++)
            {
                Marshal.StructureToPtr(accelArray[i], ptr + (i * Marshal.SizeOf<UDACCEL>()), false);
            }
            ControlProcedures.SendMessage(Hwnd, UDM.SETACCEL, (nint)accels.Length, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct UDACCEL
{
    internal int nSec;
    internal int nInc;
}
