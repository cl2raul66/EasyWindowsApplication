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
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

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
        var btn = bh.BtnGuardar;   // tipado por Source Generator
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

2. **Instala las plantillas localmente:**
   ```bash
   dotnet new install src\ProjectTemplates\EasyWinApp
   dotnet new install src\ProjectTemplates\SimpleEasyWinApp

   // Puedes instalar todas las plantillas en un solo paso
   dotnet new install src\ProjectTemplates\
   ```

> NOTA: Si tienes previamente instalada una o varias de estas plantillas y existe una actualizacion, se recomienda desintalar todas y volver a instalar.

  **Desintalaion de plantillas**
  ```bash
   dotnet new uninstall src\ProjectTemplates\EasyWinApp
   dotnet new uninstall src\ProjectTemplates\SimpleEasyWinApp

   // Puedes desintalar todas las plantillas en un solo paso
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
   dotnet new install src\ProjectTemplates\EasyWinApp
   dotnet new install src\ProjectTemplates\SimpleEasyWinApp
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
│   ├── CoreModule/                      # Fases del Fluent API, Resources, Behavior
│   ├── LayoutModule/                    # ILayoutBuilder, Grid, Stack layouts
│   ├── Share/                           # Tipos públicos (Color, Thickness, IContentBuilder)
│   │   └── Infrastructure/              # Tipos públicos técnicos ([EditorBrowsable(Never)])
│   ├── Win32ControlsModule/             # IButton, ILabel, IEdit, ICheckBox, ...
│   ├── WindowingModule/                 # IWindow, WindowPositionOnScreen
│   └── WindowsApplication.cs            # Punto de entrada del Fluent API
├── EasyWindowsApplication.Generators/   # Source Generator (EasyBehaviorGenerator)
└── ProjectTemplates/
    ├── EasyWinApp/                      # Plantilla `dotnet new easywinapp`
    └── SimpleEasyWinApp/                # Plantilla `dotnet new simpleeasywinapp`
```

## Cómo funciona

El **Source Generator** analiza tu `Layout` en tiempo de compilación: cada `.Name("BtnGuardar")` dentro de un `View<T>` o `Window` genera una propiedad tipada (`bh.BtnGuardar`) en `Behavior`. Al compilar, ya no necesitas localizar controles por string — si el nombre no coincide, el compilador te lo dice. En runtime, el `MasterRouter` centraliza el bucle de mensajes de Win32 y despacha eventos tipados (`Click`, etc.) de forma automatica.

Para usar los controles nativos del módulo Win32, activa `UseWinApi()` en Resources. **Es obligatorio**: si declaras un control (`View<T>`) sin él, el compilador lo rechaza con `EAWIN002` y no se genera ningún accessor:

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
