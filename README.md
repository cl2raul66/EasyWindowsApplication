# EasyWindowsApplication

**Un mini-framework declarativo, moderno y ultrarrápido para crear aplicaciones nativas de Windows en C#.**

`EasyWindowsApplication` acerca el rendimiento puro de Win32 —`[LibraryImport]` + **Native AOT**— a la ergonomía de C# moderno, con una **Fluent API** guiada por IntelliSense y **Source Generators** que eliminan *magic strings* y *boilerplate*. Su estructura en 4 bloques obligatorios —`Resources → Layout → Behavior → Initialize`— te guía para dar producto rápido sin perder tiempo en cableado o detalles menores.

## Características

*   **Rendimiento nativo** — Sin WebViews, sin JIT innecesario. `[LibraryImport]` + Native AOT.
*   **UX simplificada** — 4 bloques secuenciales (`Resources → Layout → Behavior → Initialize`) que reducen el esfuerzo de cableado y aceleran la entrega temprana.
*   **Fluent API declarativa** — El IntelliSense te guía de Resources a Layout a Behavior a Initialize, impidiendo equivocarte de orden.
*   **Source Generators** — Acceso tipado a controles por su nombre. Si te equivocas, falla en compilación, no en runtime.
*   **Controles Win32 nativos** — `UseWinApi()` es el *gate* obligatorio para declarar controles de `Win32ControlsModule` en `Layout`: sin él el compilador lo rechaza (EAWIN002); con él, el generador emite los accessors tipados.

## Un vistazo al código

Crear una aplicación nativa nunca fue tan limpio. Todo se divide en 4 bloques lógicos: `Resources`, `Layout`, `Behavior` e `Initialize`.

```csharp
using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

int counter = 0;

WindowsApplication
    .Resources(rd => rd.Setting(st => st.UseWinApi()))
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Mi App")
            .Dimensions(800, 600)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch => ch
                    .View<IButton>(btn => btn
                        .Name("BtnGuardar")
                        .Text("Contador: 0")
                    )
                )
            )
        )
    )
    .Behavior(bh =>
    {
        var btn = bh.BtnGuardar;   // tipado por Source Generator (View<T> wrapper)
        btn.OnClick(() =>
        {
            counter++;
            btn.Text = $"Contador: {counter}";
        });
    })
    .Initialize();
```

## Instalación y uso

El framework incluye **dos plantillas** oficiales:

| Plantilla | Short name | Ideal para |
|---|---|---|
| **EasyWinApp** | `easywinapp` | Proyectos completos organizados en secciones `Resources` / `Layout` / `Behavior` |
| **SimpleEasyWinApp** | `simpleeasywinapp` | Prototipos rápidos en un único archivo |

### Desde la consola (dotnet CLI)

1. **Clona el repositorio** y compila la solución:
   ```bash
   git clone <repo-url>
   cd EasyWindowsApplication
   dotnet build src\EasyWindowsApplication.slnx
   ```

2. **Instala las plantillas:**

   **Opción A — Paquete NuGet (recomendado para usuarios):**
   ```bash
   dotnet new install RandAMediaLabGroup.EasyWindowsApplication.Templates
   ```
   El paquete incluye ambas plantillas (`easywinapp` y `simpleeasywinapp`) y los proyectos que genera dependen de los **paquetes NuGet** del framework (`RandAMediaLabGroup.EasyWindowsApplication` y `RandAMediaLabGroup.EasyWindowsApplication.Generators`) en su última versión publicada (preview o estable).

   **Opción B — Desde la carpeta del repositorio (desarrollo):**
   ```bash
   dotnet new install src\ProjectTemplates\EasyWinApp
   dotnet new install src\ProjectTemplates\SimpleEasyWinApp

   // Puedes instalar todas las plantillas en un solo paso
   dotnet new install src\ProjectTemplates\
   ```

> NOTA: Si tienes previamente instalada una o varias de estas plantillas y existe una actualización, se recomienda desinstalar todas y volver a instalar.

  **Desinstalación de plantillas**
  ```bash
   dotnet new uninstall RandAMediaLabGroup.EasyWindowsApplication.Templates

   dotnet new uninstall src\ProjectTemplates\EasyWinApp
   dotnet new uninstall src\ProjectTemplates\SimpleEasyWinApp

    // Puedes desinstalar todas las plantillas en un solo paso
    dotnet new uninstall src\ProjectTemplates\
   ```

3. **Crea un nuevo proyecto:**
   ```bash
   dotnet new easywinapp -n MiApp               # plantilla rica (Resources/Layout/Behavior separadas)
   dotnet new easywinapp -n MiApp --Simple false  # incluye Views y Controls personalizados
   dotnet new simpleeasywinapp -n MiApp           # todo en un solo archivo
   ```

4. **Compila y ejecuta:**
   ```bash
   cd MiApp
   dotnet build
   dotnet run
   ```

### Desde Visual Studio 2026

1. **Instala las plantillas** (una vez):
   ```bash
   dotnet new install RandAMediaLabGroup.EasyWindowsApplication.Templates
   ```

2. **Archivo → Nuevo → Proyecto → Busca "Easy Win App" o "Simple Easy Win App" → Siguiente → Crear.**

## Resources (Assets, Settings y Services)

La fase `Resources` registra todo lo que tu app necesita antes de dibujar:

*   **Assets** — imágenes, iconos (`.svg` → `.ico` multi-resolución) y fuentes.
*   **Settings** — configuración embebida y persistencia (`UseWinApi`, `AppConfigFile`). `UseWinApi` es el *gate* obligatorio para usar controles Win32 nativos.
*   **Services** — contenedor de inyección de dependencias.

```csharp
WindowsApplication
    .Resources(rd => rd
        .Setting(s => s
            .UseWinApi()
            .AppConfigFile(c => c.Path("./appsettings.json").WithAutoSave())
        )
        .Services(s => s.Singleton<IMyDatabase, SqlDatabase>())
    )
    .Layout(...)
    .Initialize();
```

> La arquitectura de Resources, el pipeline SVG→ICO y las decisiones de diseño están detallados en [`CONTRIBUTING.md`](CONTRIBUTING.md) y en la **Wiki del repositorio**.

## Estructura del proyecto

```
EasyWindowsApplication/src/
├── EasyWindowsApplication/              # Framework principal
│   ├── Core/                            # Fases Fluent API, Resources, Behavior, MasterRouter (100% internal)
│   │   ├── Constants.cs / Entities.cs / Win32.cs  # Interop Win32 central ([LibraryImport], MSG/POINT/WM/WS/…)
│   │   ├── Procedures.cs                # RunMessageLoop (GetMessageW/Translate/Dispatch)
│   │   ├── UiDefaults/                  # IDefaultUiValues + UiDefaultsProvider + FontSpec (DPI scaling)
│   │   ├── Windowing/                   # WindowImpl, AlternativeWindowImpl, Procedures.CreateMainWindow/Alternative, Win32/Entities/Enums, UserControl
│   │   └── LayoutEngine/                # ILayoutBuilder, Grid/Stack/Dock layouts, ViewBuilder, ContentBuilder, ILayoutStrategy
│   ├── Common/                          # ControlActivatorRegistry + IControlActivator + INativeHandleFactory + Win32Helpers (HIWORD/LOWORD)
│   ├── Share/                           # API pública usuario (IBaseWindow→IWindow/IAlternativeWindow/IView, IWindowConfig, IChildrenBuilder con 3 overloads View<T>, View<T> sealed class + ViewBase<TSelf>, Color/Thickness/LayoutLength/GridDefinitions)
│   │   └── Infrastructure/              # Técnicos [EditorBrowsable(Never)]: ControlAccess (único tipo aquí)
│   ├── Win32ControlsModule/             # Frontend: IButton/ILabel/… (30 interfaces) + Backend: Button/Label→Core.ControlBase + Win32UiDefaults/Win32NativeHandleFactory/ControlProcedures (GDI/User32)
│   └── WindowsApplication.cs            # Punto de entrada Fluent API (devuelve Share/IApplicationLayoutPhase, static ctor → EnsureInitialized)
├── EasyWindowsApplication.Generators/   # Source Generators (EasyBehaviorGenerator EAWIN001/EAWIN002 + ControlActivatorGenerator)
└── ProjectTemplates/
    ├── EasyWinApp/                      # Plantilla `dotnet new easywinapp`
    └── SimpleEasyWinApp/                # Plantilla `dotnet new simpleeasywinapp`
```

## Cómo funciona

El **Source Generator** `EasyBehaviorGenerator` analiza tu `Layout` en tiempo de compilación: cada `.Name("BtnGuardar")` dentro de un `View<T>` (`public sealed class View<T> where T : class, IControl` con 3 overloads `Action<View<T>>` + `Func<View<T>,View<T>>` + `View(Action<IViewBuilder>)` para controles custom) o `Window`/`AlternativeWindow` genera una propiedad tipada (`bh.BtnGuardar`) en `Behavior`. En runtime, `Application.Initialize()` registra `UiDefaultsProvider.Set(new Win32UiDefaults())` antes de crear HWNDs — `Button.MeasureContent` y `ControlProcedures.GetDefaultFont` consumen esos defaults con DPI scaling (`96→dpiActual`); el `MasterRouter` (376 líneas) usa un trampolín `[UnmanagedCallersOnly]` con lookup HWND-based (`HandleRegistry.ConcurrentDictionary<nint,MasterRouter>` + 5 diccionarios adicionales) y despacha el ciclo completo `CLOSE/SIZE/MOVE/SCROLL/DPI/ERASEBKGND/CTLCOLOR/DESTROY` + eventos tipados (`WM.COMMAND→Click`); `Core/Procedures.RunMessageLoop` (`GetMessageW/Translate/Dispatch`) está separado de `Win32ControlsModule/Backend/Procedures.ControlProcedures` (capa GDI/User32); `ControlActivatorRegistry` en `Common/` (`Register<T>/Create<T>` + `RegisterFactory/TryGetFactory` + `INativeHandleFactory`, poblado en `Application.Initialize()` vía `ControlActivatorGenerator` — `IIncrementalGenerator` con `InternalsVisibleTo`, MEF compile-time) elimina el acoplamiento `Core → Win32Controls` sin `ModuleInitializer` (`CA2255`).

Para usar los controles nativos del módulo Win32, activa `UseWinApi()` en Resources. **Es obligatorio y solo compile-time** (`EAWIN002`): `SettingsBuilderImpl.UseWinApi()` retorna `this` (no-op en runtime); el Source Generator lo usa para decidir si emite accessors tipados. El mecanismo **runtime** real es `Core/UiDefaults` (`UiDefaultsProvider.Set(new Win32UiDefaults())` en `Application.Initialize()`):

```text
Win32UiDefaults (IDefaultUiValues) → UiDefaultsProvider.Set() → ControlProcedures.GetDefaultFont() / Button.MeasureContent() (DPI 96→actual)
```

Si declaras un control (`View<T>` donde `T : IControl`) sin `UseWinApi()`, el compilador lo rechaza con `EAWIN002` y no se genera ningún accessor:

```csharp
WindowsApplication
    .Resources(r => r.Setting(s => s.UseWinApi()))
    .Layout(...)
    .Behavior(bh => { ... })
    .Initialize();
```

> **Nota**: el acceso de bajo nivel al HWND/WndProc es plomería interna del framework. Con los futuros módulos MVU, lo que reciba el lambda de `.Behavior(...)` dependerá de que `UseWinApi()` esté activo — resuelto en compile-time por el Source Generator.

## ¿Desarrollas el framework?

Toda la información para compilar desde el código fuente, entender la arquitectura de módulos, el pipeline de generación de iconos y las decisiones de diseño se encuentran en [`CONTRIBUTING.md`](CONTRIBUTING.md) y en la **Wiki del repositorio**.
