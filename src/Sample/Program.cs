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
            .Title("EWA - TrayMenu Demo")
            .Dimensions(420, 200)
            .SystemTray(st => st.Tooltip("EWA Demo"))
            .Content(c => c
                .Spacing(8)
                .Padding(8)
                .Children(ch =>
                {
                    ch.View<ILabel>(lb => lb.Name("LbResult").Text("Boton muestra el mismo menu del tray."));
                    ch.View<IButton>(b => b.Name("BtnCursor").Text("Menu en cursor (testigo)"));
                    ch.View<IButton>(b => b.Name("BtnMenu").Text("Mostrar menu (Show inteligente)"));
                })
            )
        );
        ly.AlternativeWindow<IMenu>(m => m
            .Name("TrayMenu")
            .Content(c => c.Children(ch =>
            {
                ch.View<IMenuItem>(mi => mi.Name("MiShow").Text("Mostrar ventana"));
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

    // Click derecho en icono del tray -> menu anclado al icono (iconAnchor).
    bh.SystemTray.OnInputWithSpatialPosition<AlternativeTap1, OneTap>(() => bh.TrayMenu.Show());


    // Doble click izquierdo en icono del tray -> mostrar ventana principal.
    bh.SystemTray.OnInputWithSpatialPosition<MainTap, TwoTap>(() => bh.MainWindow.Show());
    //bh.SystemTray.OnInputWithSpatialPosition<MainTap, TwoTap>(() =>
    //{
    //    bh.WindowsApplication.TaskbarButtonVisibility(true);
    //    bh.MainWindow.Visibility(true);
    //    bh.SystemTray.Visibility(false);
    //});

    // Boton de la ventana principal -> Show() inteligente (issue #53):
    // con click real ancla al rect del boton; sin contexto cae a icono/cursor.
    bh.BtnMenu.OnInputWithSpatialPosition(() => bh.TrayMenu.Show());

    // Testigo explicito: siempre en cursor, no lee el contexto del disparador.
    bh.BtnCursor.OnInputWithSpatialPosition(() => bh.TrayMenu.ShowAtCursor());

    bh.MiShow.OnInputWithSpatialPosition(() => bh.MainWindow.Show());
    //bh.MiShow.OnInputWithSpatialPosition(() =>
    //{
    //    bh.WindowsApplication.TaskbarButtonVisibility(true);
    //    bh.MainWindow.Visibility(true);
    //    bh.SystemTray.Visibility(false);
    //});
    bh.MiExit.OnInputWithSpatialPosition(() => bh.WindowsApplication.Terminate());

})
.Initialize();
