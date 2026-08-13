namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface ICheckBox : IControl
{
    string Text { get; set; }
    bool Checked { get; set; }
}
