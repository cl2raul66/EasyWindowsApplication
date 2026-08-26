using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWinApp.Controls;

namespace EasyWinApp.Views;

public static class MainWindowView
{
    public static void MainWindowConfig(IWindowConfig iw) =>
        iw.Name("MainWindow")
          .Title("Easy Win App")
          .Dimensions(420, 280)
          .Position(WindowPositionOnScreen.Center)
          .Content(c => c
              .Padding(8)
              .Children(ch => ch
                  .View<IButton>(btn => btn
                      .Name("BtnIncrement")
                      .Text("Click me")
                  )
                  .View<ILabel>(lb => lb
                      .Text("Secciones + Views")
                  )
                  .View(CustomCtrl.CustomControlConfig)
              )
          );
}
