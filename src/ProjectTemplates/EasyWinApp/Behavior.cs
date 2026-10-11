using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;

namespace EasyWinApp;

public static class BehaviorConfig
{
    private static int _counter;

    public static void ConfigureBehavior(IBehaviorBuilder bh) =>
        bh.BtnIncrement.OnInputWithSpatialPosition(() =>
        {
            _counter++;
            bh.BtnIncrement.Text = $"Click: {_counter}";
        });
}
