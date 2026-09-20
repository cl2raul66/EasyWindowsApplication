using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

//WindowsApplication.Layout(ly => ly.Window()).Initialize();

WindowsApplication
    .Resources(rd => rd.Setting(st =>
    {
        st.UseWinApi();
        st.Culture(CultureInfoEnUS);
    }))
    .Layout(ly =>
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
                .Children(ch => ch
                    .View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
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
    })
    .Behavior(bh =>
    {
        bh.WindowsApplication.OnLaunched(wa =>
        {
            wa.TaskbarButtonVisibility(false);
            bh.MainWindow.Visibility(false);
            bh.SystemTray.Visibility(true);
        });
        bh.SystemTray.OnInputWithSpatialPosition<AlternativeTap1, OneTap>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithSpatialPosition<MainTap, TwoTap>(() =>
        {
            bh.SystemTray.Visibility(false);
            bh.WindowsApplication.TaskbarButtonVisibility(true);
            bh.MainWindow.Visibility(true);
        });
        bh.SystemTray.OnInputWithoutSpatialPosition<KeyMenu>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithoutSpatialPosition<Chord<KeyShift, KeyF10>>(() => { bh.MySystemTrayMenu.Show(); });
        bh.Mi1.OnClick(() =>
        {
            bh.SystemTray.Visibility(false);
            bh.WindowsApplication.TaskbarButtonVisibility(true);
            bh.MainWindow.Visibility(true);
        });
        bh.Mi2.OnClick(() =>
        {
            bh.MainWindow.Close();
        });
    })
    .Initialize();
