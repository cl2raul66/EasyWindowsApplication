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

---

## Arquitectura de módulos

El framework está organizado en módulos bajo `src/EasyWindowsApplication/`:

| Módulo | Capa | Responsabilidad |
|---|---|---|
| `CoreModule` | Backend + Frontend | Punto de entrada (`WindowsApplication`), fases del Fluent API, Resources, Behavior, MasterRouter, HandleRegistry |
| `LayoutModule` | Backend + Frontend | `ILayoutBuilder`, layouts (Grid, Stack, Dock), `IContentBuilder`, `IChildrenBuilder`, `IViewBuilder` |
| `Share` | Frontend (público) | Tipos públicos de la API de usuario: `Color`, `Thickness`, `IContentBuilder`, `LayoutLength`, `LayoutOptions` |
| `Share/Infrastructure` | Frontend (público técnico) | Tipos `public` **por razones técnicas** con `[EditorBrowsable(Never)]`: `ControlAccess`, `IconGenerator`, `ControlBase`, `View<T>` |
| `Win32ControlsModule` | Backend + Frontend | Interfaces de controles (`IButton`, `ILabel`, `IEdit`, ...) y sus implementaciones Win32 |
| `WindowingModule` | Backend + Frontend | Ventanas (`IWindow`, `IAlternativeWindow`), posicionamiento (`WindowPositionOnScreen`) |

### Principios de organización

- **`Common`** — código compartido entre módulos, pero de acceso **`internal`** (no visible para consumidores).
- **`Share`** — código compartido entre módulos, pero de acceso **`public`** (expuesto a consumidores del framework).
- **`Share/Infrastructure`** — código compartido entre módulos, `public` **por razones técnicas** (lo alcanzan el código generado, los targets de build o la herencia interna), pero **NO** parte de la API de usuario → obligatorio `[EditorBrowsable(EditorBrowsableState.Never)]`. Si un archivo ahí no lo tiene, es un bug.
- **`CoreModule/Frontend`** — interfaces de las fases del Fluent API (`IApplicationLayoutPhase` → `IApplicationPostLayoutPhase` → `IApplicationPostBehaviorPhase`) que guían por IntelliSense el orden correcto.
- **`Backend`** — implementaciones `internal` de esas interfaces.

---

## Recursos (Assets, Settings y Services)

Un **Recurso** es cualquier elemento externo al flujo de control principal que la aplicación necesita para definir su apariencia (Layout) o su lógica (Behavior). Se dividen en tres grupos:

```text
Resources(...)  |--> ASSETS   (Qué es / Cómo se ve)   --> Fuentes, Imágenes, Estilos, Cursores, Iconos
                |--> SETTINGS  (Valores de configuración) --> Defaults embebidos (Raw/appsettings.json) + persistencia en disco
                |--> SERVICES  (Qué hace / Con qué)   --> Web APIs, DBs, Repositorios, Lógica de Negocio
```

<!-- TODO: Documentar las convenciones de carpetas Resources/ y LazyAssets/ -->

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

---

## Source Generators

El proyecto `EasyWindowsApplication.Generators` contiene `EasyBehaviorGenerator`, un `IIncrementalGenerator` que:

1. **Detecta** llamadas `.Name("...")`** dentro de lambdas `View<T>(...)` y `Window(...)` en el `Layout`.
2. **Genera** `EasyBehaviorExtensions` — un *extension property* para cada nombre, tipado con la interfaz del control:
   ```csharp
   // Auto-generado (ejemplo)
   extension(IBehaviorBuilder bh) {
       public IButton BtnGuardar => ControlAccess.Get<IButton>("BtnGuardar");
   }
   ```
3. **Valida** duplicados en tiempo de compilación (`EAWIN001`: "Name '{0}' is already used...").
4. **Genera propiedades `internal static`** en la clase envolvente del `Behavior` (cuando se usa un lambda `.Behavior(bh => ...)`), para acceso directo sin el prefijo `bh.`.

### Detección de candidatos

- **Nombre**: invocaciones `.Name("string")` dentro de lambdas `View<T>()` / `Window()` / `AlternativeWindow()`.
- **Behavior**: invocaciones `.Behavior(bh => ...)` con un lambda (no method-group). Si se pasa un method-group, el enclosing-type no se detecta, pero las extensiones globales sí se generan.

---

## Pipeline de iconos SVG→ICO

Los proyectos de plantilla incluyen un `Target` de MSBuild (`GenerateAppIcon`) que:

1. Lee `Resources/AppIcon/appicon.svg`.
2. Crea un proyecto temporal con `Svg.Skia` + referencia al framework.
3. Ejecuta `IconGenerator.GenerateIco(svg, ico, new[] { 16, 32, 48, 256 })`.
4. Copia el `.ico` resultante al proyecto.
5. El `.gitignore` excluye `*.ico` generados (`src/ProjectTemplates/**/*.ico`).

> **Nota**: SkiaSharp es dependencia de *build-time* solo (no se incluye en la app final). El developer puede reemplazar el procesador implementando `IEasyImageProcessor`.

---

## Native AOT

- El framework se publica con `<PublishAot>true</PublishAot>` en las plantillas.
- Se usa `[LibraryImport]` (no `[DllImport]`) para P/Invoke de Win32, ya que en Native AOT `[DllImport]` es un antipatrón (requiere JIT en runtime).
- Los métodos de interop deben ser `partial`.

---

## Decisiones de diseño

Documenta aquí las decisiones de arquitectura relevantes. Usa ADRs (Architecture Decision Records) en `docs/adr/`.

- **Fluent API con interfaces de fase** — El IntelliSense guía el orden `Resources → Layout → Behavior → Initialize` mediante interfaces (`IApplicationLayoutPhase`, `IApplicationPostLayoutPhase`, `IApplicationPostBehaviorPhase`).
- **Controles como interfaces** (`IButton`, `ILabel`, ...) — permite que el Source Generator genere tipos débilesmente acoplados y facilita testing/mocking.
- **MasterRouter** — centraliza el bucle de mensajes (`WndProc` → `DefWindowProcW`) y despacha eventos tipados (`WM.COMMAND` → `Click`).
- **ControlAccess** — punto de acceso público para resolver controles por nombre desde el Behavior (usado por las propiedades generadas). Vive en `Share/Infrastructure` (técnico, `[EditorBrowsable(Never)]`). `SetController` se cablea en `Application.Behavior(...)`.

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
5. **Abre un Pull Request contra `develop`** e incluye en el body los marcadores de versión (`ver(core)` / `ver(gen)`) según la sección [CI/CD](#cicd).
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
   ```
   - `core` → paquete `RandAMediaLabGroup.EasyWindowsApplication`
   - `gen`   → paquete `RandAMediaLabGroup.EasyWindowsApplication.Generators`
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

- **`Svg.Skia` usa `*`** en `EasyWindowsApplication.csproj`. Si un build empieza a fallar tras una actualización de dependencias, considera *pinnear* la versión.
- **Tags de GitHub**: `v{x.y.z}` (estable) o `v{x.y.z}-preview`. Si el workflow falla al crear el tag por duplicado, elimínalo manualmente (`git push origin --delete v{tag}` o desde la interfaz) y vuelve a disparar el workflow.

---

*Este CONTRIBUTING.md es una plantilla. Sustituye los bloques `<!-- TODO -->`, `<!-- TODO: ... -->` y los enlaces de wiki cuando los documentes de Wiki y ADRs estén disponibles.*
