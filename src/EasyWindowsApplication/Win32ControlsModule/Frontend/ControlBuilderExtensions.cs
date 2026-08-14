using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public static class ControlBuilderExtensions
{
    public static T Name<T>(this T control, string name) where T : IControl
    {
        control.Name = name;
        return control;
    }

    public static T Text<T>(this T control, string text) where T : IControl, IText
    {
        control.Text = text;
        return control;
    }
}
