# Flujo General
## Flujo con secciones completas (Flujo completo)
```csharp
 WindowsApplication.Resources(...).Layout(...).Behavior(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` después de `Resources(...)`, el IntelliSense debe mostrar `Layout`.
- Cuando se pone `.` después de `Layout(...)`, el IntelliSense debe mostrar `Behavior` e `Initialize`.
- Cuando se pone `.` después de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`.
- Cuando se pone `.` después de `Initialize()`, el IntelliSense no debe recomendar nada.

## Flujo sin **Resources** y **Behavior** (Flujo mínimo)
```csharp
 WindowsApplication.Layout(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` después de `Layout(...)`, el IntelliSense debe mostrar `Behavior` e `Initialize`.
- Cuando se pone `.` después de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`. Pero no elijas Behavior, para poder hacer el flujo mínimo.
- Cuando se pone `.` después de `Initialize()`, el IntelliSense no debe recomendar nada.

## Flujo sin **Behavior** (Flujo de visualización)
```csharp
WindowsApplication.Resources(...).Layout(...).Initialize();
```
**Comportamiento del IntelliSense**
- Cuando se pone `.` después de `Resources(...)`, el IntelliSense debe mostrar `Layout`.
- Cuando se pone `.` después de `Layout(...)`, el IntelliSense debe mostrar `Behavior` e `Initialize`.
- Cuando se pone `.` después de `Behavior(...)`, el IntelliSense debe mostrar `Initialize`. Pero no elijas Behavior, para poder hacer el flujo de visualización.
- Cuando se pone `.` después de `Initialize()`, el IntelliSense no debe recomendar nada.

> **Pipeline `Initialize()`:** `Application.Initialize()` primero decide el apartamento (`0) UiApartment.RunStaOrCurrent(RunPipeline)`: si el hilo de entrada está en MTA — el default de .NET — ejecuta todo el pipeline en un `Thread` STA interno llamado `EWA-UI` y hace `Join()`; si ya es STA, corre en el hilo actual sin crear ni un hilo) y luego `RunPipeline()` ejecuta en orden: `1) UiDefaultsProvider.Set(new Win32UiDefaults())` → `2) InitCommonControlsEx(STANDARD_CLASSES)` → `3) ControlActivatorRegistry.EnsureInitialized()` → `4) new MasterRouter(registry)` → `5) foreach window: RegisterMain (CreateMainWindow + MaterializeContent + RegisterWindow + SetupSystemTrayIcon si hay `.SystemTray(...)`) o RegisterAlternative (rama `ISystemDialog`: `RegisterAlternativeDialog` — superficie sin HWND, impl COM `IFileOpenDialog`/`IFileSaveDialog` o `TaskDialogIndirect`, owner lazy `() => _mainHwnd` resuelto en `Show()`, no materializa contenido, `Name` obligatorio con throw, fallback `Name`/`Title` sueltos si no hay `ConfigureSurface`; rama `IMenu`: `MenuSurface` sin HWND + `EnsureMaterialized`; resto: CreateAlternativeWindow + MaterializeContent)` → `6) Behavior(registry + MainHwnd)` → `7) RaiseLaunched()` → `8) Procedures.RunMessageLoop()`. `UiDefaults` debe ir primero porque `GetDefaultFont`/`MeasureContent` lo leen con DPI scaling. `.Layout(...)`/`.Resources(...)` corren en el hilo de entrada (solo construyen el object graph, sin HWND ni COM); todo lo que crea HWND, menús, bandeja, behaviors y el message loop vive en `EWA-UI`.

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
> Sin strings libres: lo único válido sale del catálogo `Cultures` (generado, ~62 entradas curadas, instancia cacheada vía `GetCultureInfo`). El generator emite además `global using static` → los presets se usan pelados (`CultureInfoEnUS`) con IntelliSense `CultureInfo*`.

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
        .AlternativeWindow(aw => aw
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
        .AlternativeWindow(aw => aw
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
> `AlternativeWindow<T>` con `T = IMenu` no crea HWND: `Application.RegisterAlternative` construye un `MenuSurface` (motor `HMENU` Win32 puro, `Core/Menus/Win32MenuEngine`) y lo registra por nombre. Los items se declaran con `View<IMenuItem>` (`Name` + `Text` + `IsEnabled` + `IsChecked` + submenú vía `SubContent`); la activación se suscribe en Behavior con `OnInputWithSpatialPosition<MainTap>`; se materializan una vez (`EnsureMaterialized`) y se registran para `bh.*`. `Show()` se ancla al icono (`Shell_NotifyIconGetRect`): los triggers sin posición espacial nunca consumen la posición del mouse (el cursor es solo fallback si no hay rect).
>
> **Nota:** Existe un 4º overload `AlternativeWindow<TDialog>(Action<TDialog>)` con constraint `where TDialog : class, ISystemDialog` (en `ILayoutBuilder` y `ILayoutBuilderAfterWindow`). Los diálogos del sistema **no** usan el motor `HMENU` ni `EnsureMaterialized`; se registran vía `RegisterAlternativeDialog` (ver sección Diálogos del sistema).

```csharp
WindowsApplication.Layout(ly => ly
    .AlternativeWindow<IMenu>(m => m
        .Name("MySystemTrayMenu")
        .Content(c => c
            .Children(ch =>
            {
                ch.View<IMenuItem>(mi => mi.Name("Mi1").Text("Show main window"));
                ch.View<IMenuItemSeparator>();
                ch.View<IMenuItem>(mi => mi.Name("Mi2").Text("Exit"));
            })
        )
    )
)
.Initialize();
```

### Diálogos del sistema (`ISystemDialog`)

El 4º overload `AlternativeWindow<TDialog>(Action<TDialog>) where TDialog : class, ISystemDialog` registra superficies sin HWND que **no** usan el modelo de children ni el motor `HMENU`. En `Initialize`, `RegisterAlternativeDialog` crea la implementación concreta (COM `IFileOpenDialog`/`IFileSaveDialog` para archivos/carpetas, `TaskDialogIndirect` para task dialog), fija `OwnerProvider = () => _mainHwnd` (resuelto en `Show()`, no en registro → orden de declaración en Layout irrelevante), aplica `Name`/`Title` sueltos si no hay `ConfigureSurface`, y exige `Name` (throw si falta). En runtime, `Show()` es bloqueante y devuelve resultado tipado por diálogo.

> **Apartamento STA:** `IFileOpenDialog`/`IFileSaveDialog`/`FOS_PICKFOLDERS` son COM y exigen hilo STA (`CoInitializeEx(STA)`; el apartment COM es inmutable). Como el hilo de entrada de .NET es MTA por defecto y el framework no expone hilos al producto (ni `async`/`await`, ni `Task`, ni `SynchronizationContext`), `UiApartment.RunStaOrCurrent` re hospeda el pipeline en el `Thread` STA interno `EWA-UI`, de modo que los diálogos y su owner corren en el mismo hilo y `Show()` sigue siendo una llamada síncrona normal que devuelve el resultado tipado. Si alguien invoca `Show()` desde otra hebra (`Task.Run`/`ThreadPool`), `FileDialogCom` lo detecta (`RPC_E_CHANGED_MODE`) y lanza un error explícito en vez de colgar. `ITaskDialog` no usa COM: `TaskDialogIndirect` corre en el hilo que llama.

#### `IOpenFileDialog`

```csharp
ly.AlternativeWindow<IOpenFileDialog>(ofd => ofd
    .Name("MyOpenDialog")
    .Title("Abrir captura")
    .Filters(f => f.Children(ch =>
    {
        ch.FileFilter("Imágenes", PngFile, JpgFile);
        ch.FileFilter(AllImageFile);
    }))
    .Content(c =>
    {
        c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        c.MultiSelect(true);
    }));
```

- Estructura: `Filters` es **top-level** (estructural, siempre presente); `Content(Action<IOpenFileDialogContentBuilder>)` agrupa lo visual/comportamiento (`DefaultDirectory`, `MultiSelect`).
- `Filters(Action<IFileDialogFiltersBuilder>)`: builder espejo de `IContentBuilder.Children` → `Children(Action<IFileFilterChildrenBuilder>)` → `FileFilter(FilePattern preset)` o `FileFilter(string desc, params FilePattern[])`.
- `c.MultiSelect(bool)`: `true` → lista de rutas; `false` → una ruta (pero `FileOpenResult.FilePaths` siempre es `IReadOnlyList<string>`).
- `c.DefaultDirectory(string)`: carpeta inicial.
- Mínimo: solo `Name`/`Title` (+ `Content(c => c.MultiSelect(false))` si se quiere explicitar); sin `Filters` el OS usa su filtro por defecto.
- Resultado: `FileOpenResult(bool IsCanceled, IReadOnlyList<string> FilePaths)`.

#### `ISaveFileDialog`

```csharp
ly.AlternativeWindow<ISaveFileDialog>(sfd => sfd
    .Name("MySaveDialog")
    .Title("Guardar captura")
    .Filters(f => f.Children(ch =>
    {
        ch.FileFilter(PngFile);
        ch.FileFilter(AllFile);
    }))
    .Content(c =>
    {
        c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        c.DefaultFileName("captura");
        c.DefaultExtension(PngFile);   // FilePattern → evita string mágico; usa la extensión del primer filtro ("png")
    }));
```

- Estructura: `Filters` top-level; `Content(Action<ISaveFileDialogContentBuilder>)` con `DefaultDirectory` + `DefaultFileName(string)` + `DefaultExtension(FilePattern)` — usa la extensión del primer filtro del `FilePattern` (p.ej. `PngFile` → `"png"`), eliminando strings mágicos y alineando con `FileFilter`.
- Resultado: `FileSaveResult(bool IsCanceled, string? FilePath)` (escalar, `null` si cancelado).

#### `ISelectFolderDialog`

```csharp
ly.AlternativeWindow<ISelectFolderDialog>(sfd => sfd
    .Name("MyFolderDialog")
    .Title("Seleccionar carpeta")
    .Content(c =>
    {
        c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        c.PersistLastDirectory(false);
    }));
```

- Sin filtros ni multi-selección. Todo lo configurable vive en `Content(Action<ISelectFolderDialogContentBuilder>)`: `DefaultDirectory` + `PersistLastDirectory(bool)` (default `true`).
- `PersistLastDirectory(false)` genera un `ClientGuid` único por llamada a `Show()` (`IFileDialog::SetClientGuid`), aislando la persistencia del shell y forzando `DefaultDirectory` siempre. Con `true` se usa el comportamiento nativo (el shell recuerda la última carpeta por CLSID).
- Resultado: `FolderSelectResult(bool IsCanceled, string? FolderPath)`.

#### `ITaskDialog`

Diálogo de tarea moderno (Win32 `TaskDialogIndirect`). Dos formas de uso:

**Mínimo (un string, defaults sensatos):**
```csharp
ly.AlternativeWindow<ITaskDialog>(td => td
    .Name("ConfirmDelete")
    .Title("Confirmación")
    .Text("Esta acción no se puede deshacer."));
```
Defaults: sin icono, `Choices = Ok`, `DefaultChoice = Ok`.

**Completo (builder estructurado):**
```csharp
ly.AlternativeWindow<ITaskDialog>(td => td
    .Name("ConfirmDelete")
    .Title("Confirmación")
    .Content(c =>
    {
        c.NotificationIcon(TaskDialogIcon.Warning);
        c.PrimaryText("¿Eliminar el elemento?");
        c.SecondaryText("Esta acción no se puede deshacer.");
        c.Choices(TaskDialogChoices.Yes | TaskDialogChoices.No);
        c.DefaultChoice(TaskDialogChoice.No);
    }));
```

- Sustituto moderno de `MessageBox` (usa `TaskDialogIndirect`).
  - Detalle interno: `TaskDialogConfig` se declara con `Pack = 4` — la ABI efectiva de `TASKDIALOGCONFIG` usa packing de 4 (verificado contra el struct de CsWin32 de WinForms, `sizeof=160`); con packing natural (176, offsets 8-alineados) la API devuelve `E_INVALIDARG` silenciosamente.

| Método | Qué hace |
|---|---|
| `Text(string)` | Texto único (modo mínimo). Usa `pszMainInstruction` nativo. |
| `Content(Action<ITaskDialogContentBuilder>)` | Configuración completa. |
| `c.PrimaryText(string)` | Texto principal (grande) → `pszMainInstruction`. |
| `c.SecondaryText(string)` | Texto secundario (explicativo) → `pszContent`. |
| `c.NotificationIcon(TaskDialogIcon)` | Icono de severidad (Information/Warning/Error/Shield/None). |
| `c.Choices(TaskDialogChoices)` | Respuestas permitidas (flags: `Yes`, `No`, `Ok`, `Cancel`, `Retry`, `Close`). |
| `c.DefaultChoice(TaskDialogChoice)` | Respuesta por defecto (foco inicial). |

**Resultado:** `TaskDialogResult(bool IsCanceled, TaskDialogChoice Choice)`.

> **Nota de diseño:** `Text`/`PrimaryText`/`SecondaryText` usan `Text` porque en EWA `Content` = contenedor con `Children`. `NotificationIcon` desambigua de icono de app/control. `Choices` libera `Buttons` para futuros botones custom (`TASKDIALOG_BUTTON[]`).

#### Presets `FilePatterns`

`FilePatternsGenerator` emite ~59 presets curados (`PngFile`, `JpgFile`, `AllImageFile`, `AllVideoFile`, `AllAudioFile`, `AllDocumentFile`, `AllCodeFile`, `AllArchiveFile`, `AllFile`, etc.) como `public static readonly FilePattern` en `EasyWindowsApplication.Share.FilePatterns`. `FilePattern` es `record struct` composite: un preset compuesto **ES** un único `FilePattern` (p. ej. `AllImageFile` encapsula 11 extensiones internas), no un alias a varios. El generator emite además `global using static EasyWindowsApplication.Share.FilePatterns;` → uso pelado con IntelliSense (`PngFile`, espejo del patrón `Cultures`). Extensible sin tocar la API: añadir filas a la tabla del generator emite tokens nuevos. Filtros custom: `FileFilter("Mi filtro", PngFile, JpgFile)` o `FileFilter("Desc", miPatternCustom)`.

> **Trampa CS0121 (lambda mínima ambigua):** Una lambda solo con `Name`/`Title` es convertible tanto a `Action<IWindowConfig>` (3er overload) como a `Action<TDialog>` (4º overload) → error CS0121. Solución: parámetro tipado explícito:
> ```csharp
> ly.AlternativeWindow<IOpenFileDialog>((IOpenFileDialog ofd) => ofd
>     .Name("MinimalOpenDialog")
>     .Title("Abrir"));
> ```

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
        .BtnIncrement.OnInputWithSpatialPosition<MainTap>(() =>
        {
            counter++;
            bh.BtnIncrement.Text = $"Click: {counter}";
        })
    )
    .Initialize();
```

> Nota: Cuando usamos `OnInputWithSpatialPosition<TTrigger>` como se ve en el código anterior (Flujo en **Behavior**), es como decir `OnInputWithSpatialPosition<MainTap, OneTap>(...)` o sea que de forma predeterminada podemos decir que `OnInputWithSpatialPosition<MainTap>(...)` y `OnInputWithSpatialPosition(...)` es `OnInputWithSpatialPosition<MainTap, OneTap>(...)`.

## SystemTray y menús en **Behavior**
> El generator emite accessors tipados: `IControl` → `ControlAccess.Get<T>`, `IBaseWindow` → `GetWindow<T>`, `IViewSurface` (menús/items/**diálogos**) → `GetSurface<T>`. `bh.SystemTray` es miembro real de `IBehaviorBuilder` (nombre reservado, lazy). Triggers tipados sin nombres de dispositivo: `OnInputWithSpatialPosition<TTrigger>` (+ overload con `TCount`: `OneTap…TenTap`) y `OnInputWithoutSpatialPosition<TTrigger>`. `bh.WindowsApplication.OnLaunched` se dispara tras Behavior; `TaskbarButtonVisibility(bool)` usa `ITaskbarList`.

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
        bh.Mi1.OnInputWithSpatialPosition<MainTap, OneTap>(() =>
        {
            bh.SystemTray.Visibility(false);
            bh.WindowsApplication.TaskbarButtonVisibility(true);
            bh.MainWindow.Visibility(true);
        });
        bh.Mi2.OnInputWithSpatialPosition<MainTap, OneTap>(() =>
        {
            bh.MainWindow.Close();
        });
        // Diálogos del sistema (accessors vía GetSurface<T> como menús)
        bh.BtnIncrement.OnInputWithSpatialPosition<MainTap, OneTap>(() =>
        {
            var result = bh.MyOpenDialog.Show();
            if (!result.IsCanceled) { /* usar result.FilePaths */ }

            var save = bh.MySaveDialog.Show();
            if (!save.IsCanceled) { /* usar save.FilePath */ }

            var folder = bh.MyFolderDialog.Show();
            if (!folder.IsCanceled) { /* usar folder.FolderPath */ }

            var confirm = bh.ConfirmDelete.Show();
            if (!confirm.IsCanceled) { /* usar confirm.Choice */ }

            var minimal = bh.MinimalOpenDialog.Show();
            if (!minimal.IsCanceled) { /* usar minimal.FilePaths */ }
        });
    })
    .Initialize();
```

### Diálogos del sistema en **Behavior**

`ISystemDialog : IViewSurface` → el generator emite accessors `bh.X` vía `GetSurface<T>` (mismo path que menús/items). `Show()` es **bloqueante**; el owner es la ventana principal (`_mainHwnd`) resuelto en tiempo de llamada por `OwnerProvider = () => _mainHwnd` (no depende del orden de declaración en Layout). Patrón de consumo: `if (!result.IsCanceled) { /* usar result.*/ }` con los 4 resultados tipados:

- `FileOpenResult(IsCanceled, FilePaths)` — lista por `MultiSelect`
- `FileSaveResult(IsCanceled, FilePath)` — escalar
- `FolderSelectResult(IsCanceled, FolderPath)` — escalar
- `TaskDialogResult(IsCanceled, Choice)` — `TaskDialogChoice` escalar

> **Nota:** Los diálogos no usan triggers ni suscripción de eventos; son llamadas imperativas bloqueantes desde cualquier handler (botón, menú, tecla, etc.).

> Mapeo OS→trigger (`SystemTrayBroker`, `lParam` empaquetado v4: `LOWORD` = mensaje, `HIWORD` = uID): `WM_LBUTTONUP`/`NIN_SELECT`/`NIN_KEYSELECT`→`MainTap` (el doble-tap se codifica como cadena `MainTap` + `TwoTap` en el segundo `WM_LBUTTONUP`; `WM_LBUTTONDBLCLK` se ignora), `WM_RBUTTONUP`→`AlternativeTap1` y `WM_MBUTTONUP`→`AlternativeTap2` (ambos solo con cursor sobre el icono), `WM_MOUSEMOVE`/`NIN_POPUPOPEN`→`Hover` (flanco, reset 1s), `WM_CONTEXTMENU` por callback→`AlternativeTap1` (cursor encima) o `KeyMenu` (+ `Chord<KeyShift,KeyF10>` con Shift, cursor fuera = teclado). `LongTap`/`Holding` son **derivados** (no nativos del OS): `WM_LBUTTONDOWN` + timer 500ms → `Holding` (aún presionado); soltar tras hold → `LongTap`; soltar antes → `MainTap`. Conteo encadenado con ventana `GetDoubleClickTime()` (cap 10).
>
> Tooltip vs notificación (`ISystemTrayNotifications` = `IToolTipService` + `ITrayNotificationService`, punto de extensión para módulos): `Tooltip(text)` = hint clásico (`NIF_TIP` + `NIF_SHOWTIP`, automático al hover, silencioso); `Notify(title, message)` / `DismissNotification()` = toast real (banner + sonido + Action Center, espejo de `AppNotification`).
