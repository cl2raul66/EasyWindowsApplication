using EasyWindowsApplication;
using EasyWindowsApplication.Share;

namespace EasyWinApp;

public static class BehaviorConfig
{
    private static int _counter;

    public static void ConfigureBehavior(IBehaviorBuilder bh) =>
        bh.BtnIncrement.OnClick(() =>
        {
            _counter++;
            bh.BtnIncrement.Text = $"Click: {_counter}";
        });
}
