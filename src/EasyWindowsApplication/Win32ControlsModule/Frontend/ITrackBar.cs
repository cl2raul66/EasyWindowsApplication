namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public interface ITrackBar : IControl
{
    int Position { get; set; }
    (int min, int max) Range { get; set; }
}
