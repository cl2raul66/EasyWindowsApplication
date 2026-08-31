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

> **Pipeline `Initialize()`:** `Application.Initialize()` ejecuta en orden: `1) UiDefaultsProvider.Set(new Win32UiDefaults())` → `2) InitCommonControlsEx(STANDARD_CLASSES)` → `3) ControlActivatorRegistry.EnsureInitialized()` → `4) new MasterRouter(registry)` → `5) foreach window: CreateMainWindow/CreateAlternativeWindow + MaterializeContent + RegisterWindow` → `6) Behavior(registry)` → `7) Procedures.RunMessageLoop()`. `UiDefaults` debe ir primero porque `GetDefaultFont`/`MeasureContent` lo leen con DPI scaling.

# Flujo en **Resources**
```csharp
WindowsApplication
    .Resources(rd =>
    {
        rd.Setting(st => st
            .UseWinApi() // gate solo compile-time (EAWIN002); runtime es no-op → UiDefaultsProvider.Set(Win32UiDefaults) en Application.Initialize()
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
                    .View<IButton>(btn => btn // IButton : IControl → requiere UseWinApi() o EAWIN002
                        .Name("BtnIncrement")
                        .Text("Click me")
                    )
                )
            )
        )
    )
    .Initialize();
```

> **Nota `UseWinApi()` vs `UiDefaults`:** `UseWinApi()` solo afecta al **Source Generator** (`EAWIN002`). En runtime es `SettingsBuilderImpl.UseWinApi() => this` (no-op). El mecanismo runtime real es `Core/UiDefaults`: `Application.Initialize()` llama `UiDefaultsProvider.Set(new Win32UiDefaults())` antes de `InitCommonControlsEx` y de crear HWNDs. `ControlProcedures.GetDefaultFont()` y `Button.MeasureContent()` leen `PreferredHeight`/`FontSpec` vía `UiDefaultsProvider.Current` con DPI scaling `96→dpiActual` (ver `CONTRIBUTING.md` § Arquitectura).

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
                        .View<ILabel>(lb => lb.Name("LbMsg").Text("El mensaje es:"))
                    )
            )
        )
    )
    .Initialize();
```

## Una ventana con un control personalizado
> Usa el 3er overload `IChildrenBuilder.View(Action<IViewBuilder>)` para controles custom sin tipo genérico (`View<T> sealed class` es para `T : IControl`; `IViewBuilder` es para contenido arbitrario con `Padding/Spacing/Children`).

```csharp
WindowsApplication.Layout(ly => ly
    .Window(iw =>  iw
        .Name("MainWindow")
        .Title("Ventana - Principal")
        .Dimensions(800, 600)
        .Content(c => c
            .Children(ch => ch
                .View(cc => cc // Action<IViewBuilder> — control custom (no IControl genérico)
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

> `IChildrenBuilder` tiene 3 overloads: `View<T>(Action<View<T>>)` + `View<T>(Func<View<T>,View<T>>)` (ambos con `View<T> sealed class where T : class, IControl`) + `View(Action<IViewBuilder>)` para este caso.

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
