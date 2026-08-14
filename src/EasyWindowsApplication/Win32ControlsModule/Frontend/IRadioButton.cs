namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface IRadioButton : IControl
{
    string Text { get; set; }
    bool Checked { get; set; }
}
