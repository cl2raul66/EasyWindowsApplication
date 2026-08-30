using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

internal sealed class Win32UiDefaults : CoreUiDefaults
{
    public override FontSpec DefaultFont => FontSpec.SystemTheme;

    public override IReadOnlyDictionary<Type, ControlUiDefaults> ControlDefaults { get; } =
        new Dictionary<Type, ControlUiDefaults>
        {
            [typeof(IButton)] = new ControlUiDefaults { PreferredHeight = 30, Padding = new Thickness(10, 0) },
            [typeof(ILabel)] = new ControlUiDefaults { PreferredHeight = 23 },
            [typeof(Button)] = new ControlUiDefaults { PreferredHeight = 30, Padding = new Thickness(10, 0) },
            [typeof(Label)] = new ControlUiDefaults { PreferredHeight = 23 },
        };
}
