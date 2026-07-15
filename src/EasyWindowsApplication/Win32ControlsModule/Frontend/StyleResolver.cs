using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;

namespace EasyWindowsApplication.Win32ControlsModule.Frontend;

public static class StyleResolver
{
    public static uint Resolve<TStyle>() where TStyle : IControlStyle => TStyle.Value;
}
