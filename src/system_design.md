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

> **Pipeline `Initialize()`:** `Application.Initialize()` ejecuta en orden: `1) UiDefaultsProvider.Set(new Win32UiDefaults())` → `2) InitCommonControlsEx(STANDARD_CLASSES)` → `3) ControlActivatorRegistry.EnsureInitialized()` → `4) new MasterRouter(registry)` → `5) foreach window: RegisterMain (CreateMainWindow + MaterializeContent + RegisterWindow + SetupSystemTrayIcon si hay `.SystemTray(...)`) o RegisterAlternative (rama `IMenu`: `MenuSurface` sin HWND + `EnsureMaterialized`; resto: CreateAlternativeWindow + MaterializeContent)` → `6) Behavior(registry + MainHwnd)` → `7) RaiseLaunched()` → `8) Procedures.RunMessageLoop()`. `UiDefaults` debe ir primero porque `GetDefaultFont`/`MeasureContent` lo leen con DPI scaling.

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

## Cultura (`Culture`) — explícita o automática del OS
> Cadena de prioridad: 1) `st.Culture(...)` explícito → gana; 2) sin especificar → automático = locale del OS (el framework no toca nada); 3) `.csproj` sin efecto (`<NeutralLanguage>` solo es metadatos para `ResourceManager`; `<InvariantGlobalization>` fuerza invariante y se respeta). Si hay cultura explícita, `Initialize` fija las 4 (`CurrentCulture` + `CurrentUICulture` del hilo actual y `DefaultThreadCurrentCulture` + `DefaultThreadCurrentUICulture` para hilos futuros).
> Sin strings libres: lo único válido sale del catálogo `Cultures` (generado, ~50 curadas, instancia cacheada vía `GetCultureInfo`). El generator emite además `global using static` → los presets se usan pelados (`CultureInfoEnUS`) con IntelliSense `CultureInfo*`.

```csharp
WindowsApplication
    .Resources(rd => rd.Setting(st => st
        .UseWinApi()
        .Culture(CultureInfoEnUS) // o .Culture(Cultures.CultureInfoJaJP); sin llamada = OS
    ))
    .Layout(...)
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
                        .View<ILabel>(lb => lb.Name("LbMsg").Text("El mensaje es:"))
                    )
            )
        )
    )
    .Initialize();
```

## Una ventana con un control personalizado
> Usa el 3er overload `IChildrenBuilder.View(Action<IViewBuilder>)` para controles custom sin tipo genérico (`View<T> sealed class` es para `T : IViewSurface` — `IControl` para controles Win32 con HWND, `IMenu`/`IMenuItem` para superficies sin HWND; `IViewBuilder` es para contenido arbitrario con `Padding/Spacing/Children`).

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

> `IChildrenBuilder` tiene 4 overloads: `View<T>()` (superficie anónima, sin `Name` ni lookup `bh.*` — p. ej. separadores) + `View<T>(Action<View<T>>)` + `View<T>(Func<View<T>,View<T>>)` (ambos con `View<T> sealed class where T : class, IViewSurface`) + `View(Action<IViewBuilder>)` para este caso.

## Superficies de menú (`IMenu` / `IMenuItem`)
> `AlternativeWindow<T>` acepta cualquier `IViewSurface`. Con `T = IMenu` no se crea HWND: `Application.RegisterAlternative` construye un `MenuSurface` (motor `HMENU` Win32 puro, `Core/Menus/Win32MenuEngine`) y lo registra por nombre. Los items se declaran con `View<IMenuItem>` (`Name` + `Text` + `IsEnabled` + `IsChecked` + `OnClick` + submenú vía `SubContent`); se materializan una vez (`EnsureMaterialized`) y se registran para `bh.*`. `Show()` se ancla al icono (`Shell_NotifyIconGetRect`): los triggers sin posición espacial nunca consumen la posición del mouse (el cursor es solo fallback si no hay rect).

```csharp
WindowsApplication.Layout(ly => ly
    .AlternativeWindow<IMenu>(m => m
        .Name("MySystemTrayMenu")
        .Content(c => c
            .Children(ch =>
            {
                ch.View<IMenuItem>(i => i.Name("Mi1").Text("Mostrar ventana principal"));
                ch.View<IMenuItem>(i => i.Name("Mi2").Text("Salir"));
            })
        )
    )
)
.Initialize();
```

## SystemTray en ventana (config-time)
> `.SystemTray(...)` vive en `IWindowConfig` y recibe `Action<ISystemTray>`: `Tooltip` (hint clásico, automático al hover vía `NIF_TIP` + `NIF_SHOWTIP`) y suscripción de triggers. Notificaciones reales aparte: `Notify(title, message)` / `DismissNotification()` (toast con banner + sonido, espejo de `AppNotification`). En `RegisterMain`, `SetupSystemTrayIcon` crea el broker (ventana oculta + `Shell_NotifyIconW` + `NOTIFYICON_VERSION_4`), aplica la configuración, añade el icono y registra la superficie como `"SystemTray"`.

```csharp
WindowsApplication.Layout(ly => ly
    .Window(iw => iw
        .SystemTray(st => st.Tooltip("Mi app"))
        .Name("MainWindow")
        .Title("Easy Win App")
        .Dimensions(420, 280)
        .Content(...)
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

## SystemTray y menús en **Behavior**
> El generator emite accessors tipados: `IControl` → `ControlAccess.Get<T>`, `IBaseWindow` → `GetWindow<T>`, `IViewSurface` (menús/items) → `GetSurface<T>`. `bh.SystemTray` es miembro real de `IBehaviorBuilder` (nombre reservado, lazy). Triggers tipados sin nombres de dispositivo: `OnInputWithSpatialPosition<TTrigger>` (+ overload con `TCount`: `OneTap…TenTap`) y `OnInputWithoutSpatialPosition<TTrigger>`. `bh.WindowsApplication.OnLaunched` se dispara tras Behavior; `TaskbarButtonVisibility(bool)` usa `ITaskbarList`.

```csharp
WindowsApplication
    .Layout(ly =>
    {
        ly.Window(iw => iw
            .SystemTray(wst => wst.Tooltip(""))
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch => ch
                    .View<IButton>(btn => btn.Name("BtnIncrement").Text("Click me"))
                )
            )
        );
        ly.AlternativeWindow<IMenu>(m => m
            .Name("MySystemTrayMenu")
            .Content(c => c
                .Children(ch =>
                {
                    ch.View<IMenuItem>(i => i.Name("Mi1").Text("Mostrar ventana principal"));
                    ch.View<IMenuItem>(i => i.Name("Mi2").Text("Salir"));
                })
            )
        );
    })
    .Behavior(bh =>
    {
        bh.WindowsApplication.OnLaunched(wa => wa.TaskbarButtonVisibility(false));
        bh.MainWindow.Visibility(false);
        bh.SystemTray.Visibility(true);

        bh.SystemTray.OnInputWithSpatialPosition<MainTap, OneTap>(() => bh.MySystemTrayMenu.Show());
        bh.SystemTray.OnInputWithoutSpatialPosition<KeyMenu>(() => bh.MySystemTrayMenu.Show());
        bh.SystemTray.OnInputWithoutSpatialPosition<Chord<KeyShift, KeyF10>>(() => bh.MySystemTrayMenu.Show());
        bh.SystemTray.OnInputWithSpatialPosition<MainTap, ThreeTap>(() => { /* multi-tap */ });
    })
    .Initialize();
```

> Mapeo OS→trigger (`SystemTrayBroker`, `lParam` empaquetado v4: `LOWORD` = mensaje, `HIWORD` = uID): `WM_LBUTTONUP`/`NIN_SELECT`/`NIN_KEYSELECT`→`MainTap`, `WM_LBUTTONDBLCLK`→`MainDoubleTap`, `WM_RBUTTONUP`→`AlternativeTap1` y `WM_MBUTTONUP`→`AlternativeTap2` (ambos solo con cursor sobre el icono), `WM_MOUSEMOVE`/`NIN_POPUPOPEN`→`Hover` (flanco, reset 1s), `WM_CONTEXTMENU` por callback→`AlternativeTap1` (cursor encima) o `KeyMenu` (+ `Chord<KeyShift,KeyF10>` con Shift, cursor fuera = teclado). `LongTap`/`Holding` son **derivados** (no nativos del OS): `WM_LBUTTONDOWN` + timer 500ms → `Holding` (aún presionado); soltar tras hold → `LongTap`; soltar antes → `MainTap`. Conteo encadenado con ventana `GetDoubleClickTime()` (cap 10).
>
> Tooltip vs notificación (`ISystemTrayNotifications` = `IToolTipService` + `ITrayNotificationService`, punto de extensión para módulos): `Tooltip(text)` = hint clásico (`NIF_TIP` + `NIF_SHOWTIP`, automático al hover, silencioso); `Notify(title, message)` / `DismissNotification()` = toast real (banner + sonido + Action Center, espejo de `AppNotification`).
