using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

//WindowsApplication.Layout(ly => ly.Window()).Initialize();

WindowsApplication
    .Resources(rd => rd.Setting(st =>
    {
        st.UseWinApi();
    }))
    .Layout(static ly =>
    {
        ly.Window(iw => iw
            .SystemTray(st =>
            {
                st.Tooltip("Click here for show main window");
            })
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch =>
                {
                    ch.View<ILabel>(lb => lb.Name("LbResult").Text("No hay resultados."));
                    ch.View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    );
                })
            )
        );
        ly.AlternativeWindow<IMenu>(m => m
            .Name("MySystemTrayMenu")
            .Content(c => c
                .Children(ch =>
                {
                    ch.View<IMenuItem>(mi => mi.Name("Mi1").Text("Show main window"));
                    ch.View<IMenuItemSeparator>();
                    ch.View<IMenuItem>(mi => mi.Name("Mi2").Text("Exit"));
                })
            )
        );
        ly.AlternativeWindow<ITaskDialog>(td => td
            .Name("ConfirmExit")
            .Title("Salir")
            .Content(c =>
            {
                c.PrimaryText("¿Cerrar la aplicación?");
                c.Choices(TaskDialogChoices.Yes | TaskDialogChoices.No);
                c.DefaultChoice(TaskDialogChoice.No);
            })
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
        // Portón: única regla para la X de la principal y para Terminate()
        bh.WindowsApplication.OnTerminating(args =>
        {
            var r = bh.ConfirmExit.Show();
            if (r.IsCanceled || r.Choice is TaskDialogChoice.No)
                args.Cancel = true;
        });
        // Epílogo — sin UI
        bh.WindowsApplication.OnTerminated(() =>
            System.Diagnostics.Trace.WriteLine("[EWA] Terminated"));
        bh.SystemTray.OnInputWithSpatialPosition<AlternativeTap1, OneTap>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithSpatialPosition<MainTap, TwoTap>(() =>
        {
            bh.SystemTray.Visibility(false);
            bh.WindowsApplication.TaskbarButtonVisibility(true);
            bh.MainWindow.Visibility(true);
        });
        bh.SystemTray.OnInputWithoutSpatialPosition<KeyMenu>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithoutSpatialPosition<Chord<KeyShift, KeyF10>>(() => { bh.MySystemTrayMenu.Show(); });
        bh.Mi1.OnInputWithSpatialPosition<MainTap, OneTap>(() =>
        {
            bh.SystemTray.Visibility(false);
            bh.WindowsApplication.TaskbarButtonVisibility(true);
            bh.MainWindow.Visibility(true);
        });
        bh.Mi2.OnInputWithSpatialPosition<MainTap, OneTap>(() => bh.WindowsApplication.Terminate());
    })
    .Initialize();
