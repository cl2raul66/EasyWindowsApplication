# Flujo General
## Flujo con secciones completas (Flujo completo)
```csharp
 WindowsApplication.Resources(...).Layout(...).Behavior(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` despues de `Resources(...)`, el IntelliSense debe mostrar `Layout`.
- Cuando se pone `.` despues de `Layout(...)`, el IntelliSense debe mostrar `Behavior` y `Initialize`.
- Cuando se pone `.` despues de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`.
- Cuando se pone `.` despues de `Initialize()`, el IntelliSense no debe recomendar nada.

## Flujo sin **Resources** y **Behavior** (Flujo minimo)
```csharp
 WindowsApplication.Layout(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` despues de `Layout(...)`, el IntelliSense debe mostrar `Behavior` y `Initialize`.
- Cuando se pone `.` despues de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`.
- Cuando se pone `.` despues de `Initialize()`, el IntelliSense no debe recomendar nada.

## Flujo sin **Behavior** (Flujo de visualizacion)
```csharp
WindowsApplication.Resources(...).Layout(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` despues de `Resources(...)`, el IntelliSense debe mostrar `Layout`.
- Cuando se pone `.` despues de `Layout(...)`, el IntelliSense debe mostrar `Behavior` y `Initialize`.
- Cuando se pone `.` despues de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`.
- Cuando se pone `.` despues de `Initialize()`, el IntelliSense no debe recomendar nada.

# Flujo en **Resources**
```csharp
WindowsApplication
    .Resources(rd =>
    {
        rd.Setting(st => st
            .UseWinApi()
            .AppConfigFile(nm => nm.Path("./appsettings.json").WithAutoSave())
        );
        rd.Services(sr => sr.Singleton<IAppSettingsProvider, RegistrySettingsProvider>());
    })
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Content(c => c
                .Children(ch => ch
                    .View<Button>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
            )
        )
    )
    .Initialize();
```

# Flujo en **Layout**
## Una ventana sin componentes
```csharp
WindowsApplication.Layout(ly => ly.Window()).Initialize();
```

## Una ventana con una ventana alternativa
```csharp
WindowsApplication
    .Layout(ly => ly
        .Window(iw =>  iw
            .Name("MainWindow")
            .Title("Ventana - Principal")
            .Dimensions(800, 600)
            .Content(...)
        )
        ly.AlternativeWindow(aw => aw
            .Name("MsgErrorWindow")
            .Title("Ventana - Alternativa")
            .Dimensions(300, 200)
            .Content(...)
        )
    )
    .Initialize();
```

## Ventanas con controles
```csharp
WindowsApplication
    .Layout(ly => ly
        .Window(iw =>  iw
            .Name("MainWindow")
            .Title("Ventana - Principal")
            .Dimensions(800, 600)
            .Content(c => c
                .Children(ch => ch
                    .View<IButton>(btn => btn.Text("Click me"))
                )
            )
        )
        ly.AlternativeWindow(aw => aw
            .Name("MsgErrorWindow")
            .Title("Ventana - Alternativa")
            .Dimensions(300, 200)
            .Content(
                c => c
                    .Children(ch => ch
                        .View<ILabel>(lb => lb.Name("LbMsg").Text("El mensage es:"))
                    )
            )
        )
    )
    .Initialize();
```

## Una ventana con un control personalizado
```csharp
WindowsApplication.Layout(ly => ly
    .Window(iw =>  iw
        .Name("MainWindow")
        .Title("Ventana - Principal")
        .Dimensions(800, 600)
        .Content(c => c
            .Children(ch => ch
                .View(cc => cc
                    .Name("CustomCtrl")
                    .Content(c1 => c1
                        .Padding(8)
                        .Spacing(8)
                        .Children(ch1 => {
                            ch1.View<ILabel>(lb => lb.Text("Preciona el botón para ver el mensaje"));
                            ch1.View<IButton>(btn => btn.Text("Click me"));
                        })
                    )
                )
            )
        )
    )
    .Initialize();
```

# Flujo en **Behavior**
```csharp
WindowsApplication
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Content(c => c
                .Children(ch => ch
                    .View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
            )
        )
    )
    .Behavior(bh => bh
        .BtnIncrement.OnClick(() =>
        {
            counter++;
            bh.BtnIncrement.Text = $"Click: {counter}";
        })
    )
    .Initialize();
```
