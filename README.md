# EasyWindowsApplication

**Un mini-framework declarativo, moderno y ultrarrápido para crear aplicaciones nativas de Windows en C#.**

`EasyWindowsApplication` trae el rendimiento puro de Win32 a .NET con una **Fluent API** elegante y **Source Generators** que eliminan *magic strings* y *boilerplate*.

## ✨ Características

*   ⚡ **Rendimiento nativo** — Sin WebViews, sin JIT innecesario. `[LibraryImport]` + Native AOT.
*   🎨 **Fluent API declarativa** — Resources → Layout → Behavior → Initialize.
*   🧠 **Source Generators** — Acceso tipado a controles por su nombre. Error en compilación, no en runtime.
*   🔧 **Válvula de escape Win32** — `UseWinApi()` para acceso directo al HWND y WndProc cuando lo necesites.

## 💻 Un vistazo al código

```csharp
using EasyWindowsApplication;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

int counter = 0;

WindowsApplication
    .Layout(ly => ly
        .Window(iw => iw
            .Name("MainWindow")
            .Title("Mi App")
            .Dimensions(800, 600)
            .Position(WindowPosition.Center)
            .Content(c => c
                .Children(ch => ch
                    .View<Button>(btn => btn
                        .Position(10, 10)
                        .Dimensions(100, 32)
                        .Name("BtnGuardar")
                        .Text("Contador: 0")
                    )
                )
            )
        )
    )
    .Behavior(bh =>
    {
        var btn = bh.BtnGuardar();   // ← tipado por Source Generator
        btn.OnClick(() =>
        {
            counter++;
            btn.Text = $"Contador: {counter}";
        });
    })
    .Initialize();
```

## 🚀 Instalación y uso

### Desde la consola (dotnet CLI)

1. **Clona el repositorio** y compila la plantilla:
   ```bash
   git clone <repo-url>
   cd EasyWindowsApplication
   dotnet build src\EasyWindowsApplication.slnx
   ```

2. **Instala la plantilla localmente:**
   ```bash
   dotnet new install src\ProjectTemplates\EasyWinApp
   ```

3. **Crea un nuevo proyecto desde la plantilla:**
   ```bash
   dotnet new easywinapp -n MiApp
   cd MiApp
   dotnet build
   dotnet run
   ```

### Desde Visual Studio 2026

1. **Compila la solución** para que la plantilla esté disponible:
   ```bash
   dotnet build src\EasyWindowsApplication.slnx
   ```

2. **Instala la plantilla** (una vez):
   ```bash
   dotnet new install src\ProjectTemplates\EasyWinApp
   ```

3. **En Visual Studio 2026:** Archivo → Nuevo → Proyecto → Busca "EasyWinApp" → Siguiente → Crear.

### Para desarrollar el framework localmente

```bash
# Compilar el framework
dotnet build src\EasyWindowsApplication\EasyWindowsApplication.csproj

# Compilar y ejecutar el ejemplo
dotnet run --project src\Sample\Sample.csproj
```

## 📁 Estructura del proyecto

```
EasyWindowsApplication/
├── src/
│   ├── EasyWindowsApplication/        # Framework principal
│   ├── EasyWindowsApplication.Generators/  # Source Generator
│   ├── ProjectTemplates/EasyWinApp/   # Template para dotnet new
│   └── Sample/                        # Proyecto de ejemplo
└── README.md
```

## 🛠️ ¿Cómo funciona?

El framework usa `[LibraryImport]` para P/Invoke directo a Win32, compatible con **Native AOT**. Los **Source Generators** analizan el Layout en tiempo de compilación y generan código fuertemente tipado para el Behavior. El `MasterRouter` centraliza el bucle de mensajes de Win32 y despacha eventos tipados (`Click`, `TextChanged`, etc.) automáticamente.

Para control total sobre el HWND y WndProc, activa `UseWinApi()` en Resources:
```csharp
WindowsApplication
    .Resources(r => r.UseWinApi())
    .Layout(...)
    .Behavior(b => b.WithWin32State(ctrl => {
        var btn = ctrl.Get<Button>("BtnGuardar");
        btn.OnMessage(WM.COMMAND, (w, l) => { ... });
    }))
    .Initialize();
```
