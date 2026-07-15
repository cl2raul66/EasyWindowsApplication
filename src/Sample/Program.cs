using EasyWindowsApplication;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using System.Reflection;

// Cargar configuración por defecto desde recursos embebidos
var settingsJson = LoadEmbeddedSettings();
System.Diagnostics.Debug.WriteLine($"Settings loaded: {settingsJson ?? "none"}");

int counter = 0;

WindowsApplication
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPosition.Center)
            .Content(c => c
                .Children(ch => ch
                    .View<Button>(btn => btn
                        .Position(160, 92)
                        .Dimensions(100, 32)
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
            )
        )
    )
    .Behavior(bh => bh
        .OnClick("BtnIncrement", () =>
        {
            counter++;
            var btn = bh.Get<Button>("BtnIncrement");
            btn.Text = $"Click: {counter}";
        })
    )
    .Initialize();

static string? LoadEmbeddedSettings()
{
    var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream("Sample.Resources.Raw.appsettings.json");
    if (stream is null) return null;
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
}
