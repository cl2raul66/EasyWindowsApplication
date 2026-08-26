using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWinApp.Controls;

public static class CustomCtrl
{
    public static void CustomControlConfig(IViewBuilder cc) =>
        cc.Name("CustomCtrl")
          .Content(c => c
              .Padding(8)
              .Spacing(8)
              .Children(ch => ch
                  .View<ILabel>(lb => lb
                      .Text("Contenido del control")
                  )
              )
          );
}
