using EasyWindowsApplication;
using EasyWindowsApplication.StateManagement.MVU;

var counterState = new State<int>(0);

WindowsApplication
    .Resources(rd => rd
        .Fonts(f => f.Add("Roboto", "Assets/Roboto.ttf"))
        .Images(i => i.Add("AppLogo", "Assets/logo.png"))
    )
    .Layout(ly => ly
        .Window(w => w
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Content(c => c
                .Children(ch => ch
                    .ImmediateAction<IPushButton>(btn => btn
                        .Name("BtnIncrement")
                        .Content(c => c.Children(ch => ch.Text("Click me")))
                    )
                )
            )
        )
    )
    .Behavior(b => b
        .Bind(counterState, stateValue => b.BtnIncrement.TextPropety(stateValue.ToString()))
        .On(b.BtnIncrement.Clicked, () =>  { counterState.Value++; })
    )
    .Initialize();
