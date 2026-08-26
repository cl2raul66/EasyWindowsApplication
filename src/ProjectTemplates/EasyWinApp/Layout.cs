using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWinApp;

#if (Simple)
public static class LayoutConfig
{
    public static void ConfigureLayout(ILayoutBuilder ly) =>
        ly.Window(iw => iw
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch => ch
                    .View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
            )
        );
}
#else
using EasyWinApp.Views;

public static class LayoutConfig
{
    public static void ConfigureLayout(ILayoutBuilder ly) =>
        ly.Window(MainWindowView.MainWindowConfig)
          .AlternativeWindow(MsgErrorWindowView.MsgErrorWindowConfig);
}
#endif
