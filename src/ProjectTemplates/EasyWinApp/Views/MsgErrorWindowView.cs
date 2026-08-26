using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWinApp.Views;

public static class MsgErrorWindowView
{
    public static void MsgErrorWindowConfig(IWindowConfig iw) =>
        iw.Name("MsgErrorWindow")
          .Title("Error")
          .Dimensions(320, 180)
          .Content(c => c
              .Padding(8)
              .Children(ch => ch
                  .View<ILabel>(lb => lb
                      .Text("Algo ha fallado.")
                  )
              )
          );
}
