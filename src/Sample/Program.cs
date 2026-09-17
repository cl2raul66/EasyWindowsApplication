using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

//WindowsApplication.Layout(ly => ly.Window()).Initialize();

WindowsApplication
    .Resources(rd => rd.Setting(st => { 
        st.UseWinApi();
        st.Culture(new System.Globalization.CultureInfo("en-us"));
    }))
    .Layout(ly =>
    {
        ly.Window(iw => iw
                .SystemTray(st =>
                {
                    st.Tooltip("");
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
        bh.SystemTray.OnInputWithSpatialPosition<Hover>(() => { bh.SystemTray.TooltipShow(); });
        bh.SystemTray.OnInputWithSpatialPosition<MainTap, OneTap>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithoutSpatialPosition<KeyMenu>(() => { bh.MySystemTrayMenu.Show(); });
        bh.SystemTray.OnInputWithoutSpatialPosition<Chord<KeyShift, KeyF10>>(() => { bh.MySystemTrayMenu.Show(); });
    })
    .Initialize();
