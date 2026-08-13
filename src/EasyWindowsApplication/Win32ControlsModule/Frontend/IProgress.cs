namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IProgress : IControl
{
    int Value { set; }
    (int min, int max) Range { set; }
    int CurrentValue { get; }
    (int min, int max) Range32 { set; }
    int Delta { set; }
    int Step { set; }
    int BarColor { set; }
    int BkColor { set; }
    void StepIt();
    void SetMarquee(bool enable, int speed = 0);
    int GetStep();
    int GetBarColor();
    int GetBkColor();
}
