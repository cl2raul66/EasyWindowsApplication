using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

WindowsApplication
    .Resources(rd => rd.Setting(st =>
    {
        st.UseWinApi();
    }))
    .Layout(static ly =>
    {
        ly.Window(iw => iw
            .Name("MainWindow")
            .Title("EWA - Demo")
            .Dimensions(525, 494)
            .SystemTray(st => st.Tooltip("EWA - Demo"))
        );
        ly.AlternativeWindow<IMenu>(m => m
            .Name("TrayMenu")
            .Content(c => c.Children(ch =>
            {
                ch.View<IMenuItem>(mi => mi.Name("MiShowMainWin").Text("Go to main window"));
                ch.View<IMenuItemSeparator>();
                ch.View<IMenuItem>(mi => mi.Name("MiExit").Text("Salir"));
            }))
        );
    })
.Behavior(bh =>
{
    bh.WindowsApplication.OnLaunched(wa =>
    {
        wa.TaskbarButtonVisibility(false);
        bh.MainWindow.Visibility(false);
        bh.SystemTray.Visibility(true);
    });

    bh.SystemTray.OnInputWithSpatialPosition<MainTap, TwoTap>(() => bh.MainWindow.Show());

    bh.SystemTray.OnInputWithSpatialPosition<AlternativeTap1>(() => bh.GalleryMenu.Show());

    bh.MiExit.OnInputWithSpatialPosition(() => bh.WindowsApplication.Terminate());

    bh.MiShowMainWin.OnInputWithSpatialPosition(() => bh.MainWindow.Show()); 
})
.Initialize();
