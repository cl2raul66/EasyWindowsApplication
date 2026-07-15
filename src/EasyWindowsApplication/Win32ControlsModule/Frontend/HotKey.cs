using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public sealed class HotKey : ControlBase<HotKey>
{
    public (byte virtualKey, byte modifiers) HotKeyCombination
    {
        get
        {
            nint result = ControlProcedures.SendMessage(Hwnd, HKM.GETHOTKEY, 0, 0);
            byte lo = (byte)(result & 0xFF);
            byte hi = (byte)((result >> 8) & 0xFF);
            return (lo, hi);
        }
        set
        {
            nint packed = (nint)((value.modifiers << 8) | value.virtualKey);
            ControlProcedures.SendMessage(Hwnd, HKM.SETHOTKEY, packed, 0);
        }
    }

    public void SetRules(int invalidModComb, int invalidKeyComb)
    {
        ControlProcedures.SendMessage(Hwnd, HKM.RULES, (nint)invalidModComb, (nint)invalidKeyComb);
    }
}
