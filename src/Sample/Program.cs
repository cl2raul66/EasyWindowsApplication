using EasyWindowsApplication;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

int counter = 0;

WindowsApplication
    .Resources(rd => rd.Setting(st => st.UseWinApi()))
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch =>
                {
                    ch.View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    );
                    ch.View<ILabel>(lb => lb.Name("LbShowIncrement").Text("0 Increment"));
                })
            )
        )
    )
    .Behavior(bh =>
    {
        bh.BtnIncrement.OnClick(() =>
        {
            counter++;
            bh.LbShowIncrement.Text = $"{counter} Increment";
        });
    })
    .Initialize();
