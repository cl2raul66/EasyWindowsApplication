using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal interface IDefaultUiValues
{
    WindowUiDefaults MainWindow { get; }
    WindowUiDefaults AlternativeWindow { get; }

    FontSpec DefaultFont { get; }
    Color DefaultForeground { get; }
    Color DefaultBackground { get; }
    Thickness DefaultPadding { get; }
    int DefaultControlSpacing { get; }
    bool EnableVisualStyles { get; }

    IReadOnlyDictionary<Type, ControlUiDefaults> ControlDefaults { get; }
}
