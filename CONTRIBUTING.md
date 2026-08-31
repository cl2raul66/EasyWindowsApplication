# Contributing a EasyWindowsApplication

> **Plantilla de guía para colaboradores y mantenedores del framework.**
>
> Este documento está pensado para desarrolladores que contribuyen al propio framework (`src/EasyWindowsApplication/`). Si eres **consumidor** del framework (lo usas para crear aplicaciones), consulta la **Wiki del repositorio**.

---

## Índice

1. [Requisitos previos](#requisitos-previos)
2. [Compilar desde el código fuente](#compilar-desde-el-codigo-fuente)
3. [Arquitectura de módulos](#arquitectura-de-modulos)
4. [Recursos (Assets, Settings y Services)](#recursos-assets-settings-y-services)
5. [Source Generators](#source-generators)
6. [Pipeline de iconos SVG→ICO](#pipeline-de-iconos-svg-ico)
7. [Native AOT](#native-aot)
8. [Decisiones de diseño](#decisiones-de-diseno)
9. [Cómo contribuir](#como-contribuir)
10. [Testing](#testing)
11. [CI/CD](#cicd)

---

## Requisitos previos

<!-- TODO: Completar con las versiones específicas de .NET SDK, Windows SDK, etc. -->

- **.NET 10 SDK** (o superior)
- **Windows SDK** (Windows 10/11)
- **Visual Studio 2026** (recomendado) o `dotnet CLI`
- (Opcional) SkiaSharp para el pipeline de imágenes

---

## Compilar desde el código fuente

```bash
git clone <repo-url>
cd EasyWindowsApplication

# Compilar toda la solución
dotnet build src\EasyWindowsApplication.slnx

# Compilar solo el framework
dotnet build src\EasyWindowsApplication\EasyWindowsApplication.csproj

# Compilar y ejecutar una plantilla de ejemplo
dotnet run --project src\ProjectTemplates\SimpleEasyWinApp\SimpleEasyWinApp.csproj
```

### Plantillas (desarrollo)

Para reinstalar o desinstalar las plantillas sin recordar comandos, ejecuta el gestor interactivo:

```bash
.\devtools\install-templates.ps1
```

El menú permite reinstalar desde el repositorio (`src\ProjectTemplates`) o desde un paquete `.nupkg` local, desinstalar, y salir.

---

## Arquitectura de módulos

El framework está organizado en módulos bajo `src/EasyWindowsApplication/`:

| Módulo | Capa | Responsabilidad |
|---|---|---|
| `Core` | `internal` (100%) | Núcleo del framework: `WindowsApplication` → `Share/IApplicationLayoutPhase`, `Application` (pipeline `Initialize()` en 5 fases), `MasterRouter` (trampolín `[UnmanagedCallersOnly]` + `HandleRegistry` `ConcurrentDictionary<nint,MasterRouter>` + 6 diccionarios adicionales; ciclo `WM.CLOSE/SIZE/MOVE/ACTIVATE/SCROLL/DPI/ERASEBKGND/CTLCOLOR/DESTROY`), `HandleRegistry` (hwnd→control/window, parent→children), `ControlBase`, `IconGenerator`, `Procedures.RunMessageLoop()` (`GetMessageW/TranslateMessage/DispatchMessageW`), `Constants.cs` (227 líneas: `WS/WS_EX/CS/CW/WM/SPI/WMSIZE/WA/MONITOR/DT/SCROLLBAR/SIF/BN/EN/SW/GWL/ICC/SWP`), `Entities.cs` (`MSG/POINT/NMHDR/SCROLLINFO/…`), `Win32.cs` (~40 P/Invoke `[LibraryImport]` para Native AOT), `LayoutModels.cs` (`WindowModel/ContentModel/ViewModel`), `ResourcesDictionaryImpl.cs` (`SettingsBuilderImpl/UseWinApi` no-op + `AppConfigFile/Services` builders) |
| `Core/UiDefaults` | `internal` | Sistema centralizado de defaults con DPI scaling: `IDefaultUiValues`, `UiDefaultsProvider` (singleton `CoreFallbackDefaults` → `Set(Win32UiDefaults)` en `Application.Initialize()` antes de registrar ventanas), `FontSpec` (`SystemTheme`), `ControlUiDefaults`/`WindowUiDefaults`/`CoreUiDefaults` (`PreferredHeight/Padding/Background`), `DefaultUiValuesExtensions`. Acoplamiento crítico: `Button.MeasureContent()` lee `UiDefaultsProvider.Current.GetFor<IButton>().PreferredHeight` y escala `96→dpiActual`; `ControlProcedures.GetDefaultFont()` lee `UiDefaultsProvider.Current.DefaultFont` → `CreateFontFromSpec` |
| `Core/Windowing` | `internal` | `WindowImpl`, `AlternativeWindowImpl`, `Procedures` (`CreateMainWindow` con `WS_OVERLAPPEDWINDOW` + `CreateAlternativeWindow` con `WS_POPUP|CAPTION|SYSMENU`, registro `HandleRegistry.RegisterRouter` + `WndProcTrampoline`), `UserControl`, `Win32.cs` (P/Invoke parcial `[LibraryImport]`), `Entities.cs` (`MSG/POINT/NMHDR/…`), `Enums.cs`, `Constants.cs` (stub: "Window-specific constants are in Core") |
| `Core/LayoutEngine` | `internal` | `LayoutEngine` (batching `DeferWindowPos` anti-flicker), `ILayoutStrategy`/`ILayoutable`, `LayoutBuilderImpl`, `ContentBuilderImpl`, `WindowConfigBuilder`, `ChildrenBuilderImpl` (usa `ControlActivatorRegistry`), `ViewBuilderImpl`, estrategias `GridLayoutStrategy`/`DockLayoutStrategy`/`HorizontalStackLayoutStrategy`/`VerticalStackLayoutStrategy` |
| `Common` | `internal` | `ControlActivatorRegistry` (`Register<T>/Create<T>`, `RegisterFactory<T>/TryGetFactory/CreateFactory/TryGetFactoryForControl`, `Shared` + `EnsureInitialized()` + `partial RegisterGeneratedActivators()`), `IControlActivator` (`RegisterActivators`), `INativeHandleFactory` (`CreateHandle(parentHwnd, control, registry)`), `Win32Helpers` (`HIWORD/LOWORD` usado por `MasterRouter`). Rompe ciclo `Common ↔ Core` sin `ModuleInitializer` (`CA2255`); usa `IIncrementalGenerator` + `InternalsVisibleTo` |
| `Share` | `public` | API usuario: jerarquía ventanas `IBaseWindow` → `IWindow`/`IAlternativeWindow`/`IView`, `IWindowConfig`/`IContentBuilder`/`ILayout`/`ILayoutBuilderAfterWindow.AlternativeWindow()`/`IViewBuilder` (controles custom), `ViewBase<TSelf>` (`Share/View.cs`, base para controles custom ej. `IpAddress`) + `View<T> sealed class where T : class, IControl` (`Name/Margin/Padding/Width/Height/Background/Dock/OnClick/Text` + `PendingName` y lista `Action<T>`), `IChildrenBuilder` (3 overloads: `View<T>(Action<View<T>>)` + `View<T>(Func<View<T>,View<T>>)` + `View(Action<IViewBuilder>)`), primitivas `LayoutLength`/`GridDefinitions`/`Thickness`/`Color`/`LayoutOptions`/`DockPosition`/`WindowPositionOnScreen`/`ScrollBarVisibility`, eventos `CancelEventArgs/WindowMoved/Resized/Resizing`, `IResourcesDictionary`/`IApplicationLayoutPhase` (Type-State) |
| `Share/Infrastructure` | `public` técnico | `[EditorBrowsable(Never)]`: `ControlAccess` (punto usado por `EasyBehaviorGenerator` vía `ControlAccess.Get<T>(name)` / `GetWindow<T>`) — único tipo en este namespace |
| `Win32ControlsModule` | Frontend `public` / Backend `internal` | **Frontend:** 30 interfaces `IControl` → `IButton/ILabel/ICheckBox/IComboBox/…/ITreeView/IUpDown` + `ControlBuilderExtensions` + `StyleInterfaces` + clase `IpAddress : ViewBase<IpAddress>` · **Backend:** `Button`/`Label` → `Core.ControlBase` (con `MeasureContent` + DPI scaling acoplado a `UiDefaultsProvider`), `Win32UiDefaults` (`IDefaultUiValues` concreto registrado en `Application.Initialize()`), `Win32NativeHandleFactory` (`INativeHandleFactory`), `Win32ControlActivator` (`IControlActivator`, `internal`, auto-registrado vía `ControlActivatorGenerator` + `EnsureInitialized()`), `Procedures.ControlProcedures` (capa GDI/User32: `GetDefaultFont/CreateFontFromSpec/GetDpiForWindowSafe/SendMessage/InvalidateDefaultFont/CreateControl`), `Win32.cs`/`Constants.cs`/`Entities.cs` del módulo |
| `Generators` | `Analyzer` | `EasyBehaviorGenerator` (detecta `.Name("…")` en `View<T>/Window/AlternativeWindow`, genera `EasyBehaviorExtensions` + `EAWIN001` duplicados + `EAWIN002` gate `UseWinApi()`, FQN `Share` + `GetTypeByMetadataName`) + `ControlActivatorGenerator` (descubre `IControlActivator` vía `AllInterfaces`, genera `ControlActivatorRegistrations.g.cs` con `new Win32ControlActivator().RegisterActivators(Shared)`, `InternalsVisibleTo`) |

### Principios de organización

- **`Common`** — código compartido `internal` (no visible para consumidores). Contiene `ControlActivatorRegistry` + `INativeHandleFactory`/`Win32Helpers` (evita reflexión, factorías genéricas + `HIWORD/LOWORD`).
- **`Share`** — API `public` para consumidores. Todo lo que devuelve `WindowsApplication` (`IApplicationLayoutPhase` etc.) vive aquí. `ViewBase<TSelf>` y `View<T> sealed class` están en `Share/View.cs` (no en `Infrastructure`).
- **`Share/Infrastructure`** — `public` por razones técnicas (`[EditorBrowsable(Never)]`). Solo `ControlAccess` permanece aquí; `ViewBase<TSelf>` está en `Share`, `ControlBase`/`IconGenerator` migraron a `Core/`.
- **`Core/**`** — 100% `internal`. Nada `public` aquí (si aparece `public` es bug, debe ser `internal` o moverse a `Share`). Implementaciones puras. `Core/Constants.cs` es el archivo central de constantes Win32 (227 líneas); `Core/Windowing/Constants.cs` es un stub.
- **`Core/UiDefaults`** — 100% `internal`. Singleton `UiDefaultsProvider` con 7 archivos (`IDefaultUiValues/FontSpec/ControlUiDefaults/WindowUiDefaults/CoreUiDefaults`). Se inicializa en `Application.Initialize()` antes de crear HWNDs; `Button.MeasureContent` y `ControlProcedures.GetDefaultFont` dependen de él (DPI scaling).
- **`Win32ControlsModule/Frontend`** — `public` interfaces controles (30 tipos); `Backend` — `internal` implementaciones que heredan `Core.ControlBase` y se registran vía `IControlActivator` + `Win32UiDefaults`/`Win32NativeHandleFactory`/`ControlProcedures`.

---

## Recursos (Assets, Settings y Services)

Un **Recurso** es cualquier elemento externo al flujo de control principal que la aplicación necesita para definir su apariencia (Layout) o su lógica (Behavior). Se dividen en tres grupos:

```text
Resources(...)  |--> ASSETS   (Qué es / Cómo se ve)   --> Fuentes, Imágenes, Estilos, Cursores, Iconos
                |--> SETTINGS  (Valores de configuración) --> Defaults embebidos (Raw/appsettings.json) + persistencia en disco
                |--> SERVICES  (Qué hace / Con qué)   --> Web APIs, DBs, Repositorios, Lógica de Negocio
```

<!-- Convenciones de carpetas Resources/ documentadas abajo; LazyAssets/ no existe -->

### Estructura de directorios Resources

```text
Resources/
├── AppIcon/
│   └── appicon.svg        # → .ico multi-resolución (16/32/48/256)
├── Cursors/
│   └── Arrow.cur
├── Splash/
│   └── splashscreen.svg
├── Images/
│   └── logo.svg           # → PNG, incrustado como RCDATA
├── Fonts/
│   └── Roboto.ttf         # → cargada desde RAM vía Win32 sin instalar
├── Raw/
│   └── appsettings.json   # → SETTINGS por defecto, auto-detectado
```

> **Nota `UseWinApi()` vs `UiDefaults`:** `SettingsBuilderImpl.UseWinApi()` es un **marcador compile-time** (`EAWIN002`): en runtime retorna `this` (no-op). El Source Generator lo usa para decidir si emite `EasyBehaviorExtensions` para controles `IControl` de `Win32ControlsModule`; sin `UseWinApi()` el compilador rechaza `.View<IButton>` etc. El mecanismo **runtime** real es `Core/UiDefaults`: `Application.Initialize()` llama `UiDefaultsProvider.Set(new Win32UiDefaults())` antes de `InitCommonControlsEx` y de crear HWNDs. `ControlProcedures.GetDefaultFont()` consume `UiDefaultsProvider.Current.DefaultFont` (`FontSpec.SystemTheme` → `LOGFONT` → `HFONT` con DPI) y `Button.MeasureContent()` consume `UiDefaultsProvider.Current.GetFor<IButton>().PreferredHeight` escalado `96→dpiActual`. No confundir gate de compilación con configuración de UI en runtime.

---

## Source Generators

El proyecto `EasyWindowsApplication.Generators` contiene `EasyBehaviorGenerator`, un `IIncrementalGenerator` que (FQN actualizados a `EasyWindowsApplication.Share.*` + robustez `GetTypeByMetadataName`):

1. **Detecta** llamadas `.Name("...")`** dentro de lambdas `View<T>(...)` (ahora `View<T>` es `Share/View<T>` `sealed class`) y `Window(...)` / `AlternativeWindow(...)` en el `Layout`.
2. **Genera** `EasyBehaviorExtensions` — un *extension property* para cada nombre, tipado con la interfaz del control (FQN `global::EasyWindowsApplication.Share.IWindow` / `IAlternativeWindow` / `global::EasyWindowsApplication.Share.IBehaviorBuilder`):
   ```csharp
   // Auto-generado (ejemplo)
   extension(IBehaviorBuilder bh) {
       public IButton BtnGuardar => ControlAccess.Get<IButton>("BtnGuardar");
   }
   ```
3. **Valida** duplicados en tiempo de compilación (`EAWIN001`: "Name '{0}' is already used...").
4. **Valida el gate `UseWinApi()`** en tiempo de compilación (`EAWIN002`): si se declara un control (`View<T>` cuyo tipo hereda de `IControl` de `Win32ControlsModule`) sin activar `UseWinApi()`, emite un error y **no genera** ningún accessor de controles. Las ventanas no se gatean.
5. **Genera propiedades `internal static`** en la clase envolvente del `Behavior` (cuando se usa un lambda `.Behavior(bh => ...)`), para acceso directo sin el prefijo `bh.`.

### Detección de candidatos

- **Nombre**: invocaciones `.Name("string")` dentro de lambdas `View<T>()` / `Window()` / `AlternativeWindow()`.
- **Behavior**: invocaciones `.Behavior(bh => ...)` con un lambda (no method-group). Si se pasa un method-group, el enclosing-type no se detecta, pero las extensiones globales sí se generan.
- **`UseWinApi()`**: invocaciones `.UseWinApi()` (0 args) cuya verificación semántica confirme el símbolo `ISettingsBuilder.UseWinApi` (solo existe dentro de `.Resources(...).Setting(...)`).
- **Control**: invocaciones `View<T>(lambda)` cuyo `T` hereda de `EasyWindowsApplication.Win32ControlsModule.Frontend.IControl`.

> **Naming**: referenciar un control desde `Behavior` requiere `.Name()` explícito; sin `.Name()` no se genera accessor (el control sigue funcionando en el Layout por HWND). El auto-naming fue evaluado y **rechazado** (deriva semántica silenciosa al editar el Layout).

- **Robustez FQN**: `ISettingsBuilder` se resuelve vía `Compilation.GetTypeByMetadataName("EasyWindowsApplication.Share.ISettingsBuilder")` + `SymbolEqualityComparer.Default` (no string comparison). Si `GetTypeByMetadataName` retorna `null`, el gate `UseWinApi` no se activa.
- **View<T>**: el generador detecta `View<T>` genérico con `T : IControl`; `IWindow`/`IAlternativeWindow` usan FQN `Share` (antes `WindowingModule.Frontend`).

---

## Pipeline de iconos SVG→ICO

Los proyectos de plantilla incluyen un `Target` de MSBuild (`GenerateAppIcon`) que:

1. Lee `Resources/AppIcon/appicon.svg`.
2. Crea un proyecto temporal con `Svg.Skia` + referencia al framework.
3. Ejecuta `IconGenerator.GenerateIco(svg, ico, new[] { 16, 32, 48, 256 })`.
4. Copia el `.ico` resultante al proyecto.
5. El `.gitignore` excluye `*.ico` generados (`src/ProjectTemplates/**/*.ico`).

> **Nota**: SkiaSharp es dependencia de *build-time* solo (no se incluye en la app final). El developer puede reemplazar el procesador implementando `IEasyImageProcessor`.

> **Alineación de versiones**: el proyecto temporal `IcoGen` debe referenciar la **misma** versión de `Svg.Skia` que el framework (`EasyWindowsApplication.csproj` usa `*` flotante). La línea se declara como `Version="%2A"` en los tres `.csproj` con el target (`Sample`, `EasyWinApp`, `SimpleEasyWinApp`): `%2A` es el escapado MSBuild de `*`, necesario porque un `*` literal en un `Include` de ItemGroup se interpreta como *wildcard* y MSBuild descarta el item. El `IcoGen.csproj` generado recibe `Version="*"`, de modo que ambos resuelven al mismo `Svg.Skia` en el mismo build. Si se fija la versión del framework, hay que actualizar también las 3 líneas del `IcoGen`; un desajuste produce `System.IO.FileNotFoundException` al ejecutar `IcoGen` (código `-532462766`).

---

## Native AOT

- El framework se publica con `<PublishAot>true</PublishAot>` en las plantillas.
- Se usa `[LibraryImport]` (no `[DllImport]`) para P/Invoke de Win32, ya que en Native AOT `[DllImport]` es un antipatrón (requiere JIT en runtime).
- Los métodos de interop deben ser `partial`.

---

## Decisiones de diseño

Documenta aquí las decisiones de arquitectura relevantes. Usa ADRs (Architecture Decision Records) en `docs/adr/`.

- **Fluent API con interfaces de fase (Type-State)** — El IntelliSense guía el orden `Resources → Layout → Behavior → Initialize` mediante interfaces (`IApplicationLayoutPhase`, `IApplicationPostLayoutPhase`, `IApplicationPostBehaviorPhase`) ahora en `Share/` (antes `CoreModule/Frontend`). `WindowsApplication` devuelve `IApplicationLayoutPhase` público. `IApplicationPostLayoutPhase` expone tanto `Behavior()` como `Initialize()` para flujos mínimos (`Resources→Layout→Initialize` o `Layout→Initialize`). `Application.cs` tiene overloads sin parámetro `Layout()`/`Behavior()` que retornan `this` para encadenamiento condicional. `WindowsApplication` tiene `static constructor` que llama `ControlActivatorRegistry.EnsureInitialized()` (inicialización perezosa; segundo llamado en `Application.Initialize()` es no-op por flag `_inited`).
- **Controles como interfaces** (`IButton`, `ILabel`, ...) — permite que el Source Generator genere tipos débilesmente acoplados y facilita testing/mocking. `Button`/`Label` heredan `Core.ControlBase` (antes `Share.Infrastructure.ControlBase`) y contienen lógica DPI (`MeasureContent` escala `PreferredHeight` 96→dpi actual vía `UiDefaultsProvider`).
- **MasterRouter** — 376 líneas: centraliza el bucle de mensajes con trampolín `[UnmanagedCallersOnly(CallConvs=[Stdcall])]` + lookup HWND-based (`HandleRegistry.ConcurrentDictionary<nint,MasterRouter>`), `try/catch` que evita tumbar proceso, ciclo completo `WM.CLOSE/ENTERSIZEMOVE/EXITSIZEMOVE/SIZE/MOVE/ACTIVATE/HSCROLL/VSCROLL/MOUSEWHEEL/SETTINGCHANGE/DPICHANGED/ERASEBKGND/CTLCOLOR/DESTROY` (typed dispatch `WM.COMMAND→Click` + raw handlers + cleanup `Unregister/PostQuitMessage` + `WM.ERASEBKGND` con `LayoutGroup` backgrounds + `WM.CTLCOLOR*` con control brushes), despacha eventos tipados (`WM.COMMAND` → `Click`). El delegado estático sobrescrito fue eliminado (evita GC). Paint/Theme/DPI/scroll/lifecycle separados del GDI.
- **HandleRegistry** — No es solo `ConcurrentDictionary`. Contiene `ConcurrentDictionary<nint,MasterRouter>` (routers) + 5 diccionarios: `hwnd→WeakReference<IControl>`, `name→WeakReference<IControl>`, `name→IBaseWindow`, `hwnd→IBaseWindow`, `parent→List<nint> children`. Métodos: `TrackChildWindow/UnregisterWindowControls/RegisterWindow/UnregisterWindow/UnregisterWindowByHwnd/GetWindowByHwnd/GetByName/GetWindow/AllControlHandles`.
- **ControlAccess / ControlActivatorRegistry / ControlActivatorGenerator** — `ControlAccess` sigue en `Share/Infrastructure` (usado por `EasyBehaviorGenerator`). `ControlActivatorRegistry` en `Common/` (genérico `Register<T>/Create<T>` + `RegisterFactory<T>/TryGetFactory/CreateFactory/CreateFactoryFor/TryGetFactoryForControl` + `INativeHandleFactory` + `Shared` + `EnsureInitialized()` + `partial RegisterGeneratedActivators()`) rompe acoplamiento `Core → Win32ControlsModule` sin `ModuleInitializer` (`CA2255` en `Library`). `ControlActivatorGenerator` (`IIncrementalGenerator` en `Generators`, descubre `IControlActivator` en `Compilation` vía `GetTypeByMetadataName` + `AllInterfaces`, genera `ControlActivatorRegistrations.g.cs` con `new Win32ControlActivator().RegisterActivators(Shared)` y futuros plugins; usa `<InternalsVisibleTo Include="EasyWindowsApplication.Generators"/>` para mantener `internal`.
- **View<T> sealed class** — `Share/View<T>` `public sealed class where T : class, IControl` (no `struct`; semántica de referencia en heap, GC, identidad por referencia) con lista `Action<T>` + fluents (`Name/Margin/Padding/Width/Height/Background/Dock/OnClick/Text`). Triple overload en `IChildrenBuilder`: `View<T>(Action<View<T>>)` + `View<T>(Func<View<T>,View<T>>)` + `View(Action<IViewBuilder>)` para controles custom sin tipo genérico, instanciando `View<T>(control)` vía `ControlActivatorRegistry`.
- **`UseWinApi()` como bifurcación explícita** — gate **solo compile-time** (`EAWIN002`): `SettingsBuilderImpl.UseWinApi()` retorna `this` (no-op en runtime). El Source Generator decide si emite accessors tipados. El mecanismo runtime real es `Core/UiDefaults` (`UiDefaultsProvider.Set(new Win32UiDefaults())` en `Application.Initialize()` antes de `InitCommonControlsEx` y registro de ventanas). `UseWinApi()` y `UiDefaults` son conceptos separados. `IconGenerator` migró `Share/Infrastructure` → `Core/` (ahora `Core.IconGenerator`, IcoGen usa `EasyWindowsApplication.Core`).
- **UiDefaults y DPI** — `Core/UiDefaults/` (7 archivos) + `Backend/Win32UiDefaults` + `Backend/Controls.cs` (`Button.MeasureContent` escala `PreferredHeight` vía `UiDefaultsProvider.Current.GetFor<IButton>()` y `GetDpiForWindow` 96→actual; `ControlProcedures.GetDefaultFont` lee `FontSpec` vía `UiDefaultsProvider.Current.DefaultFont` → `CreateFontFromSpec` → `GetDpiForSystemSafe`). Ver diagrama en [Arquitectura de módulos](#arquitectura-de-modulos).
- **Separación Procedures** — `Core/Procedures.cs` (`RunMessageLoop`: `while(GetMessageW>0){Translate/Dispatch}`) pertenece al núcleo; `Core/Windowing/Procedures.cs` (`CreateMainWindow/CreateAlternativeWindow` con `RegisterClassExW` + `WndProcTrampoline`) pertenece a windowing; `Win32ControlsModule/Backend/Procedures.cs` (`ControlProcedures`: `GetDefaultFont/CreateFontFromSpec/GetDpiForWindowSafe/SendMessage/CreateControl`) es la capa GDI/User32. Mezclar message loop con GDI es el error Win32 más común — esta separación debe preservarse.
- **AlternativeWindow** — ahora funcional: `Procedures.CreateAlternativeWindow` registra clase `WS_POPUP|CAPTION|SYSMENU` y `HandleRegistry.RegisterRouter(hwnd, router)`; `Application.RegisterAlternative` crea hwnd real, aplica BackgroundBrush y `Show()`.

---

## Cómo contribuir

Todo trabajo (funcionalidad, ajuste, bugfix) comienza con un **issue** creado con las plantillas del repositorio (`.github/ISSUE_TEMPLATE/`). El proceso es **manual** y sigue este flujo:

1. **Crea el issue** en la web usando la plantilla correspondiente (`feat:`, `bug:`, etc.).
2. **Crea la rama desde `develop`** en la página del issue → barra lateral *Development* → **Create a branch**. La rama nace con el formato `{número}-{slug}` y base `develop`.
3. **Bájala localmente**:
   ```bash
   git fetch origin
   git checkout {número}-{slug}
   ```
4. **Trabaja y haz push** con commits en Conventional Commits (`feat:`, `fix:`, ...).
5. **Abre un Pull Request contra `develop`** e incluye en el body los marcadores de versión (`ver(core)` / `ver(gen)` / `ver(template)`) según la sección [CI/CD](#cicd).
6. **Mergea a `develop`** → `dev-release.yml` publica el preview automáticamente. Los usuarios prueban y dan feedback, que se canaliza en nuevos issues.

Reglas:

- Trabaja sobre `develop`, nunca directamente sobre `main`.
- Incluye tests relevantes.
- Asegura que `dotnet build src\EasyWindowsApplication.slnx` pase.

---

## Testing

<!-- TODO: Completar con el setup de testing una vez definido -->

- El `Sample` (cuando exista) sirve como proyecto de validación manual de funcionalidades.
- Ejecuta: `dotnet run --project src\ProjectTemplates\SimpleEasyWinApp\SimpleEasyWinApp.csproj`

---

## 11. CI/CD

Este repositorio publica paquetes NuGet automáticamente mediante GitHub Actions. Existen dos workflows:

| Workflow | Rama origen | Resultado |
|---|---|---|
| `.github/workflows/dev-release.yml` | PR *mergeada* a `develop` | Publica versión **`-preview`** a [nuget.org](https://www.nuget.org/) |
| `.github/workflows/prod-release.yml` | PR *mergeada* a `main` | Publica versión **estable** a [nuget.org](https://www.nuget.org/) |

### Cómo disparar un release

1. **Trabaja en `develop`** con commits usando Conventional Commits (`feat:`, `fix:`, etc.).
2. **Abre un PR a `develop`** — incluye en el *body* los marcadores de versión:
   ```text
   ver(core): 0.1.0-preview
   ver(gen): 0.1.0-preview
   ver(template): 0.1.0-preview
   ```
   - `core` → paquete `RandAMediaLabGroup.EasyWindowsApplication`
   - `gen`   → paquete `RandAMediaLabGroup.EasyWindowsApplication.Generators`
   - `template`   → paquete `RandAMediaLabGroup.EasyWindowsApplication.Templates` (template pack `easywinapp` / `simpleeasywinapp`)
3. **Al *mergear*** a `develop`, el workflow `dev-release.yml` publica automáticamente el preview a NuGet y crea un *GitHub Release* con tag `v0.1.0-preview`.
4. **Para promover a estable**, abre un PR de `develop` → `main` con el **mismo** *body* (marcadores con `-preview`). Al *mergear*, `prod-release.yml` publica la versión estable y elimina el `-preview` del tag.

### Publicación automática: Trusted Publishing (OIDC)

La autenticación contra nuget.org usa **Trusted Publishing** (OIDC), no API keys de larga duración. GitHub Actions intercambia un token OIDC de corta duración por una API key temporal válida 1 hora mediante `NuGet/login@v1`.

Setup manual (una sola vez):

1. En **nuget.org** → tu usuario → **Trusted Publishing** → **+ Create**, crear dos políticas:
   - **Dev Policy** → Owner: tu cuenta NuGet / Repository Owner `cl2raul66` / Repository `EasyWindowsApplication` / Workflow File `dev-release.yml` / Environment `dev`.
   - **Prod Policy** → igual pero Workflow File `prod-release.yml` / Environment `prod`.
2. En **GitHub** → `Settings → Environments`: crear los environments `dev` y `prod`.
3. En **GitHub** → `Settings → Secrets and variables → Actions` → crear el secret:

| Secret | Propósito |
|---|---|
| `NUGET_USER` | Tu nombre de usuario de nuget.org (el *profile name*, no el email). Lo usa `NuGet/login@v1`. |

> **Limpieza (solo tras verificar un publish OK con OIDC):** eliminar el secret `NUGET_API_KEY` (si existiera) de GitHub y revocar las API keys en nuget.org (`tu usuario → API Keys`). Es irreversible: no lo hagas antes de confirmar que el publish OIDC funciona.

### Notas para mantenedores

- **`Svg.Skia` usa `*`** en `EasyWindowsApplication.csproj`. Si un build empieza a fallar tras una actualización de dependencias, considera *pinnear* la versión — en ese caso actualiza también las 3 líneas del proyecto `IcoGen` (ver [Pipeline de iconos SVG→ICO](#pipeline-de-iconos-svg-ico)).
- **Tags de GitHub**: `v{x.y.z}` (estable) o `v{x.y.z}-preview`. Si el workflow falla al crear el tag por duplicado, elimínalo manualmente (`git push origin --delete v{tag}` o desde la interfaz) y vuelve a disparar el workflow.

---

*Este CONTRIBUTING.md es una plantilla. Sustituye los bloques `<!-- TODO -->`, `<!-- TODO: ... -->` y los enlaces de wiki cuando los documentes de Wiki y ADRs estén disponibles.*
