- Por el momento usaremos un solo proyecto para la biblioteca de clases.
- Para mejor rendimiento y futuros ajustes de rendimiento, la biblioteca se publica con `Native AOT`.
- En el pasado, para `Win32.cs` se usaria `[DllImport("user32.dll")]`. **En Native AOT esto es un antipatrón** porque requiere generación de código en tiempo de ejecución (JIT). Se debe usar el atributo `[LibraryImport]`. Esto obliga a que tus métodos en `Win32.cs` sean `partial`.
- Al consumir las bibliotecas nativas de `Win32`, usara modo inseguro.
- Un proyecto CLI de C# para uso interno, con el objetivo de ver cada funcionalidad desarrollada o en desarrollo, este lo llamaremos `Sample`.
- `EasyWindowsApplication.Generators` es un proyecto de apoyo a la biblioteca `EasyWindowsApplication`. Mi idea es usar el poder de Source Generator para crear propiedades con el atributo `Name` declarados, donde el generador de código analiza en que contexto aparece  en tiempo de compilación y genera una estructura fuertemente tipada (un strongly-typed alias).
- `Common` es para código compartido y/o repetitivo entre los módulos, este siempre debe ser interno (internal).
- `Share` es para código compartido y/o repetitivo entre los módulos (internamente), pero también externamente (public), para ser usados por los desarrolladores que emplean la biblioteca.
- `Share/Infrastructure` es para código compartido entre módulos que es `public` por razones técnicas (código generado, targets de build o herencia interna), pero NO parte de la API de usuario → debe llevar `[EditorBrowsable(EditorBrowsableState.Never)]`.
- `WindowsApplication.cs` el punto de entrada del Fluent Api, debe ser simple, limpio, exponer lo que se necesita para la escribir el flujo evitando que se vuelva un fichero de codigo monolitico, esa es su unica funcion.
- La aplicación se construye mediante una Fluent API declarativa dividida estrictamente en secciones secuenciales. Esto garantiza la separación de responsabilidades:
  ```csharp
  WindowsApplication.Resources(...).Layout(...).Behavior(...).Initialize();
  ```
  Tambien se puede escribir asi:
  ```csharp
  WindowsApplication.Resources(...).Layout(...).Initialize();
  WindowsApplication.Layout(...).Initialize();
  ```

- **`WindowsApplication`**: El nodo raíz que representa la aplicación.
- **`Resources`**: Aquí se registran los assets (fuentes, imágenes, estilos, etc), contenedor de inversión de control (IoC) y servicios (inyección de dependencias). Es donde se registra todo lo recursos que luego se usarán en el `Layout` y/o en el `Behavior`. Por lo tanto, aquí nada se dibuja ni se ejecuta aún. La definición formal de mi concepto de "Recurso" (segun mi vision), estructurada para validar la arquitectura y guiar al usuario en las buenas prácticas de rendimiento y el uso de generadores de código (*Source Generators*).

  **Definición Arquitectónica de "Recurso":**
  Un **Recurso** es *cualquier elemento, dato, configuración o lógica de negocio externa al flujo de control principal, requerido por la aplicación para definir su apariencia (Layout) o su lógica (Behavior)*. No se limita a la sección .rsrc del ejecutable, tambien es el **Contenedor IoC / Inyección de Dependencias** de la app, dividiéndose en tres grandes grupos:

  ```text
  Resources(...)  |--> ASSETS (Qué es / Cómo se ve)         --> Fuentes, Imágenes, Estilos, Cursores, Iconos, etc
                  |--> SETTINGS (Valores de configuración)  --> Defaults embedidos (Raw/appsettings.json) + persistencia en disco
                  |--> SERVICIOS (Qué hace / Con qué)       --> Web APIs, DBs, Repositorios, Lógica de Negocio, etc
  ```

  * La idea de rendimiento en esta seccion: Para que mantenga el rendimiento nativo y eficiente de Win32, el desarrollador debe entender la naturaleza de lo que registra en el diccionario de recursos (**ASSETS**, **SETTINGS** y **SERVICIOS**). La documentación y/o tipos de registro deberían forzar estas buenas prácticas.
  * Si el usuario borra un recurso o cambia su Name en el diccionario, el código del `Layout` y/o `Behavior` que lo usaba no compilará.
  * Reglas de MSBuild (`.targets`), que le dicen al compilador de .NET: "Busca en la carpeta Resources, toma los archivos y conviértelos automáticamente en recursos nativos de Win32 antes de compilar".

    **Representación de la estructura del directorio Resources**
    ```text
    Resources/
    ├── AppIcon/
    │   └── appicon.svg
    ── Cursors/
    │   └── Arrow.cur
    │   └── Pointer.cur
    │   └── Loading.ani
    ├── Splash/
    │   └── splashscreen.svg
    ├── Images/
    │   └── logo.svg
    ├── Fonts/
    │   └── Roboto.ttf
    ├── Raw/
    │   └── appsettings.json
    ```
  * El Source Generator, lee los nombres de los archivos en esas carpetas y genera las propiedades fuertemente tipadas para el IntelliSense.
  * **Resources/AppIcon/**: El framework debe saber que el archivo aquí (ej. `.svg`, `.png`, `.jpg`, `.bmp` seran convertido a `.ico`, con la exepcion de `.ico`) debe compilarse estrictamente como un recurso Win32 estándar (`RT_ICON` con `ID 1`). Esto garantiza que el Explorador de Windows muestre el ícono en el `.exe`. Recomendamos usar fichero `.svg`, por sus caracteristicas de vertores.

    **El proceso de empaquetado del ícono funcionaria así:**

    a. **Archivo base (SVG u otros):**
      Se colocas un archivo en `Resources/AppIcon`.

    b. **Conversión a PNG en varias resoluciones:**
      - Si usas **SVG**, al compilar se rasteriza automáticamente a múltiples tamaños de **PNG** (16x16, 32x32, 48x48, 256x256, etc.), que son los tamaños estándar que Windows utiliza para íconos.
      - Esto asegura que el ícono se vea bien en diferentes escalas (barra de tareas, menú inicio, mosaicos, etc.).

    c. **Generación del `.ico`:**
      - Se debe generar un **archivo `.ico`** que contenga todas las resoluciones (los ficheros **PNG**) empaquetadas.
      - Se combina los PNG generados en un único `.ico` durante el proceso de compilación.
      - Ese `.ico` es el que finalmente se registra como el ícono de la aplicación en Windows.

    > NOTA: Todos los ficheros de imagen deben convertise a **PNG**.

  * **Pipeline de compilación de imágenes (build-time):** El framework convierte automáticamente imágenes vectoriales (SVG) a recursos nativos Win32 durante la compilación, sin dependencias en tiempo de ejecución. Esto se implementa en el proyecto `EasyWindowsApplication.BuildTasks` como una MSBuild Task que se inyecta vía NuGet (`.targets`). El pipeline:

    **Arquitectura del proyecto `EasyWindowsApplication.BuildTasks`:**
    ```text
    EasyWindowsApplication.BuildTasks/
    ├── ImageProcessingTask.cs         ← MSBuild Task (hereda de Microsoft.Build.Utilities.Task)
    ├── SkiaSharpImageProcessor.cs     ← implementación por defecto (SkiaSharp)
    ├── IcoEncoder.cs                  ← genera el formato .ico (estándar ICO, ~100 líneas)
    ├── IEasyImageProcessor.cs         ← interfaz pública para agnosticismo
    └── EasyWindowsApplication.targets ← reglas MSBuild (se inyectan automáticamente)
    ```

    **Interfaz para agnosticismo:**
    ```csharp
    public interface IEasyImageProcessor
    {
        byte[] ConvertToIco(string sourcePath, int[] sizes);
        byte[] ConvertToPng(string sourcePath, int width, int height);
    }
    ```

    **Soporte para todos los entornos de build (.NET 10+ / MSBuild 18.0+):**
    ```xml
    <!-- En msbuild.exe/VS (.NET Framework): out-of-process, evita file locking de DLLs nativas -->
    <UsingTask TaskName="ImageProcessingTask"
        Runtime="NET"
        TaskFactory="TaskHostFactory"
        AssemblyFile="...\EasyWindowsApplication.BuildTasks.dll"
        Condition="'$(MSBuildRuntimeType)' == 'Full'" />

    <!-- En dotnet build (.NET): in-process, máximo rendimiento -->
    <UsingTask TaskName="ImageProcessingTask"
        Runtime="NET"
        AssemblyFile="...\EasyWindowsApplication.BuildTasks.dll"
        Condition="'$(MSBuildRuntimeType)' == 'Core'" />
    ```

    **Cómo se activa:** El `.targets` del NuGet detecta los archivos en `Resources/AppIcon/` y ejecuta la tarea antes de `CoreCompile`. El developer solo coloca un SVG en la carpeta y el framework hace el resto. SkiaSharp es la implementación por defecto y solo se necesita en build-time (no se incluye en la app final). El developer puede reemplazar el procesador implementando `IEasyImageProcessor` y configurando la propiedad `$(EasyWinImageProcessor)` en el `.csproj`.

  * **Resources/Cursors/**: El framework los compila como recursos estándar de Win32 (RT_CURSOR o RT_ANICURSOR).
  * **Resources/Splash/** y **Resources/Images/**: El framework asume que son `RCDATA` (incrustados en la RAM). Win32 no entiende nativamente un `.svg`,`.png`, `.jpg` o un `.bmp`, en su sección de recursos estándar, por lo que incrustarlos como `RCDATA` (bytes crudos), pienso, que es la forma correcta y de máximo rendimiento para que los lea desde la memoria.
  * **Resources/Fonts/**: El framework los incrusta como `RCDATA`. En tiempo de ejecución, tu framework usará la API nativa de Win32 para cargar la fuente desde la memoria RAM sin instalarla en el sistema operativo.
  * **Resources/Raw/**: `RCDATA` puro. Ideal para archivos `JSON` de configuración inicial, diccionarios de traducción base, etc. El archivo `appsettings.json` ubicado aquí recibe tratamiento especial: el framework lo reconoce automáticamente como los valores por defecto del grupo **SETTINGS**, sin necesidad de declaración explícita en la Fluent API. Los archivos de localizacion, pueden usar cualquier convención de nombre (ej. `strings.en.json`, `strings.es.json`) para organización por idioma. El framework es agnóstico, el developer elige qué recurso cargar según el locale activo. No hay infraestructura de satélite, no hay impacto en el framework.
  * La carpeta hermana, **LazyAssets/**: El framework no incrusta estos archivos en el `.exe`. En su lugar, le dice al compilador que los copie a la carpeta de salida (`bin/Release/LazyAssets` o `bin/Debug/LazyAssets`). El Source Generator los lee y crea el alias, pero internamente el framework sabrá que debe usar lectura de disco (I/O) bajo demanda cuando el usuario los solicite.

- **`Behavior`**: Enrutamiento de eventos y lógica. Gracias a los Source Generators, los nombres dados en el `Layout` (ej. `BtnGuardar`) aparecen aquí fuertemente tipados, eliminando los *Magic Strings*.Soporta dos modalidades:
* Behavior<IMvuState> (Implícito/Por defecto): Para un flujo moderno basado en estados y señales, ideal para la mayoría de aplicaciones.
* Behavior<IWin32State>: Válvula de escape para desarrolladores avanzados. Expone el HWND y permite interceptar mensajes directamente en el WndProc (ej. WM_COMMAND, WM_PAINT) para cada control declarado en el Layout, ofreciendo el mismo nivel de control que C/C++ nativo. Es plomería interna (`IWin32State` es `internal`), se cablea automáticamente al activar `UseWinApi()`.
- **`Initialize()`**: El nodo terminal que bloquea el hilo, cede el control al `CoreModule` y arranca el bucle de mensajes de Win32. En el flujo, es el nodo terminal de la Fluent API. No debe confundirse con un hook de ciclo de vida ni con las acciones del IDE (Build, Run). Su semántica es: en este punto la declaración está completa, todo ha sido definido y el sistema está habilitado para arrancar. Es el equivalente conceptual de `Build()` en el patrón Builder clásico, con la diferencia de que el nombre refleja que los Source Generators ya han actuado en tiempo de compilación, por lo que al llegar aquí no se construye nada, simplemente se lanza lo que ya está construido.
- Plantilla para Visual Studio, esta debe servir como un "esqueleto" (scaffolding) para crear las carpetas y poner los archivos placeholder.

## Convenciones de Código

- Cada funcionalidad va en un módulo y dentro de este se divide en dos directorios `Frontend` y `Backend` donde todo el código de frontend puede ser público o interno, pero el de backend, siempre va a ser interno.
- En cada directorio `Backend` y en `Common` contendrán estos ficheros `Constants.cs`, `Enums.cs`, `Entities.cs`, `Procedures.cs`, `Win32.cs`.
- `Constants.cs` si las metes en `Entities.cs`, se mezclará la definición de datos (structs) con los números mágicos.

  **Ejemplo de Constants.cs**

  ```csharp
  internal static class WS {
      public const uint CHILD = 0x40000000;
      public const uint VISIBLE = 0x10000000;
  }
  internal static class BS {
      public const uint PUSHBUTTON = 0x00000000;
  }

  // El código fluye de forma natural, tal como se diseñó en C/C++:
  Win32.CreateWindowEx(
      0, "BUTTON", "Guardar",
      WS.CHILD | WS.VISIBLE | BS.PUSHBUTTON, // Cero casteos, máxima legibilidad
      0, 0, 100, 30, hWndParent, IntPtr.Zero, hInstance, IntPtr.Zero);
  ```
- `Enums.cs` Debes usarlos **solo cuando el valor representa un conjunto cerrado y mutuamente excluyente**, y no una bandera de bits (flag) o un mensaje genérico.

  **Ejemplos perfectos para `enums` en Win32:**

  1. **Comandos de Mostrar Ventana (`ShowWindow`):**
    ```csharp
    internal enum ShowWindowCommand : int {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_MAXIMIZE = 3
    }
    // La firma de Win32 lo exige estricto:
    [LibraryImport("user32.dll")]
    internal static partial bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);
    ```

  2. **Resultados de un MessageBox:**
    ```csharp
    internal enum MessageBoxResult : int {
        IDOK = 1,
        IDCANCEL = 2,
        IDYES = 6,
        IDNO = 7
    }
    ```
- `Entities.cs` contendrá todas las estructuras de datos que usaran en `Win32.cs` para comunicarse ya sea pasarlas como argumentos o para obtenerlas como resultados.
- `Win32.cs` todas las llamadas a Win32 necesarias para el módulo.
- `Procedures.cs` contendrá todas las funciones que ajustadas para que el Frontend las use o sea es como el Core de la biblioteca.
- Si una función o estructura del `Backend` se repite en otro modulo, se debe pasar a `Common`.
- En `Frontend`, puede existir `Enums.cs` y `Dtos.cs` como parte de la interfaz pública, estos tambien podrian estar en `Share` si se repiten en otros modulos.
- En `Common`, puede existir `Helpers.cs` con funciones de ayuda para el desarrollo, como por ejemplo, funciones de extensión para facilitar la lectura o escritura de código.

## Arquitectura de Orquestación (CoreModule)

El framework utiliza un diseño de "Sistema Nervioso Central".

- **El `CoreModule` es el Orquestador:** Es el único dueño del bucle de mensajes de Win32 (`GetMessage`). No dibuja controles ni conoce la lógica de negocio. Su única función es inicializar la app, mantener un registro de identificadores (`HandleRegistry`) y enrutar los mensajes del sistema operativo hacia los módulos correspondientes.
- **Los Módulos Visuales son "Órganos Tontos":** Módulos como `ImmediateAction` o `DataEntry` solo saben cómo dibujarse a sí mismos llamando a Win32 y cómo avisar que fueron interactuados. No se comunican entre sí directamente, todo pasa por el `CoreModule` a través de la interfaz `IComponentReceiver`.

## Estructura del Proyecto

```text
EasyWindowsApplication/
│
├── .gitignore
├── LICENSE
├── README.md
└── src/
    ├── EasyWindowsApplication.slnx
    │
    ├── EasyWindowsApplication.Generators/
    │   ├── Analyzers/
    │   │   └── FluentApiAnalyzer.cs (Lee lo que el usuario escribe)
    │   ├── Emitters/
    │   │   ├── Win32BoilerplateEmitter.cs (Genera el código de registro HWND)
    │   │   └── MvuBindingEmitter.cs (Genera el puente si el usuario usó MVU)
    │
    ├── EasyWindowsApplication.BuildTasks/
    │   ├── ImageProcessingTask.cs      ← MSBuild Task (SkiaSharp + formato .ico)
    │   ├── SkiaSharpImageProcessor.cs  ← implementación por defecto
    │   ├── IcoEncoder.cs               ← generación del estándar ICO
    │   ├── IEasyImageProcessor.cs      ← interfaz pública (agnosticismo)
    │   └── EasyWindowsApplication.targets  ← reglas MSBuild
    │
    ├── EasyWindowsApplication/
    │       ├── WindowsApplication.cs
    │       │
    │       ├── Share/
    │       │   ├── IBaseWindow.cs
    │       │   └── IAppSettingsProvider.cs
    │       │
    │       ├── Common/
    │       │   ├── Constants.cs
    │       │   ├── Enums.cs
    │       │   ├── Entities.cs
    │       │   ├── IAssociatedWindow.cs
    │       │   ├── IIndependentWindow.cs
    │       │   ├── ISubordinateWindow.cs
    │       │   ├── Win32.cs
    │       │   └── Procedures.cs
    │       │
    │       ├── CoreModule/
    │       │   ├── Frontend/
    │       │   │   ├── IApplicationHost.cs
    │       │   │   └── IComponentReceiver.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       ├── HandleRegistry.cs
    │       │       └── MasterRouter.cs
    │       │
    │       ├── WindowingModule/
    │       │   ├── Frontend/
    │       │   │   ├── IWindow.cs              (alias público de IIndependentWindow)
    │       │   │   ├── IAlternativeWindow.cs   (alias público de IAssociatedWindow)
    │       │   │   ├── IView.cs                (alias público de ISubordinateWindow)
    │       │   │   ├── IStackLayout.cs
    │       │   │   ├── IDockStackLayout.cs
    │       │   │   ├── IVerticalStackLayout.cs
    │       │   │   └── IHorizontalStackLayout.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── DataEntryModule/
    │       │   ├── Frontend/
    │       │   │   ├── ITextBox.cs
    │       │   │   ├── IRichEditBox.cs
    │       │   │   ├── IAutoSuggestBox.cs
    │       │   │   ├── IPasswordBox.cs
    │       │   │   ├── IMaskedTextBox.cs
    │       │   │   └── IDataEntry.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── ExclusionarySelectorModule/
    │       │   ├── Frontend/
    │       │   │   ├── ICheckBox.cs
    │       │   │   ├── IComboBox.cs
    │       │   │   ├── IToggleSwitch.cs
    │       │   │   ├── IToggleButton.cs
    │       │   │   ├── IToggleSplitButton.cs
    │       │   │   ├── ICalendarDatePicker.cs
    │       │   │   ├── ICalendarView.cs
    │       │   │   ├── IDatePicker.cs
    │       │   │   ├── ITimePicker.cs
    │       │   │   └── IExclusionarySelector.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── NavigationModule/
    │       │   ├── Frontend/
    │       │   │   ├── IScrollView.cs
    │       │   │   ├── IPipsPager.cs
    │       │   │   ├── ISlider.cs
    │       │   │   ├── ITabView.cs
    │       │   │   ├── IBreadcrumbBar.cs
    │       │   │   ├── ISelectorBar.cs
    │       │   │   ├── ISemanticZoom.cs
    │       │   │   └── INavigation.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── BrowserDataModule/
    │       │   ├── Frontend/
    │       │   │   ├── IItemsView.cs
    │       │   │   ├── ITreeView.cs
    │       │   │   ├── INavigationView.cs
    │       │   │   └── IBrowserData.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── PassiveInteractionModule/
    │       │   ├── Frontend/
    │       │   │   ├── ITextBlock.cs
    │       │   │   ├── IRichTextBlock.cs
    │       │   │   ├── IImage.cs
    │       │   │   ├── IAnimatedVisualPlayer.cs
    │       │   │   ├── IInfoBadge.cs
    │       │   │   ├── IInfoBar.cs
    │       │   │   ├── IProgressBar.cs
    │       │   │   ├── IProgressRing.cs
    │       │   │   └── IPassiveInteraction.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── GroupContainersModule/
    │       │   ├── Frontend/
    │       │   │   ├── ICommandBar.cs
    │       │   │   ├── IMenuBar.cs
    │       │   │   └── IGroupContainers.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── ImmediateActionModule/
    │       │   ├── Frontend/
    │       │   │   ├── IPushButton.cs
    │       │   │   ├── ISplitButton.cs
    │       │   │   ├── IHyperlinkButton.cs
    │       │   │   └── IImmediateAction.cs
    │       │   │
    │       │   └── Backend/
    │       │       ├── Constants.cs
    │       │       ├── Enums.cs
    │       │       ├── Entities.cs
    │       │       ├── Win32.cs
    │       │       └── Procedures.cs
    │       │
    │       ├── StateManagement/
    │       │   ├── MVU/
    │       │   │   ├── State.cs
    │       │   │   └── Signal.cs
    │       │
    │
    └── Sample/
```

## Layout

```csharp
.Window()                        // IWindow implícito (ventana principal)
.AlternativeWindow()             // IAlternativeWindow (ventana secundaria)
.View()                          // IView (contenido visual dentro de una ventana)

.Content<IVerticalStackLayout>(...)
.Content<IHorizontalStackLayout>(...)
.Content<IDockStackLayout>(...)

Layout(ly => ly
    .Window(iw =>  iw
        .Name("MainWindow")
        .Title("Mi App Ultrarrápida")
        .Dimensions(800, 600)
        .Content(c => c.Sapcing(8).Children(ch => ch.View(...)))
    )
    ly.AlternativeWindow(aw => aw
        .Name("MsgError")
        .Dimensions(300, 200)
        .Content(c => c.Sapcing(8).Children(ch => ch.View(...)))
    )
)

.Content(c => c
    .Sapcing(8)
    .View(sw => sw
        .Name("BtnGuardar")
        .Content(c => c.Children(ch => ch.Text("Guardar Datos")))
    )
)
```

- `IIndependentWindow`, `IAssociatedWindow` y `ISubordinateWindow` heredan de `IBaseWindow`.
- Los nombres `IIndependentWindow`, `IAssociatedWindow` y `ISubordinateWindow` son nombres internos que mantienen coherencia con la terminología de Win32 y no deben exponerse directamente en la Fluent API. Los alias públicos para el desarrollador final son:

  * `IIndependentWindow` → `IWindow`
  * `IAssociatedWindow` → `IAlternativeWindow`
  * `ISubordinateWindow` → `IView`

  > NOTA: `IWindow` es la ventana principal. `IAlternativeWindow` es cualquier ventana secundaria (diálogos, paneles flotantes). `IView` es cualquier contenido visual dentro de una ventana, sin importar si ocupa toda la superficie o solo una fracción, incluyendo controles personalizados y vistas que se comportan como páginas.

- `IStackLayout` clase base de los tipos de layout (diseno de la disposicion de los elementos), contiene propiedades como `Sapcing` y `Children`.
- `IVerticalStackLayout`, `IHorizontalStackLayout` y `IDockStackLayout` heredan de `IStackLayout`.
- La seccion `Layout` solo admiten un `IWindow` y múltiples `IAlternativeWindow`, estos deben ir después de `IWindow`. El flujo es que solo podrás escribir `Window` y después de este podrás poner todos los `AlternativeWindow` que quieras, para ayudar al desarrollador y al compilador podremos usar `IAlternativeWindow`, quedando así:
  ```csharp
  ly.AlternativeWindow(aw => aw
          .Name("NotificationMsg")
          .Dimensions(300, 200)
          .Content(c => c.Sapcing(8).Children(ch => ch.View(...)))
      );
  ly.AlternativeWindow(aw => aw
          .Name("ReconnectMsg")
          .Dimensions(300, 200)
          .Content(c => c.Sapcing(8).Children(ch => ch.View(...)))
      )
  ```
- Para los `IBaseWindow` la estructura fluida es `Window.{estilos, imágenes, servicios o sea todo lo que se necesite del Resources}.{Propiedades propias según el tipo de ventana (IIndependentWindow, IAssociatedWindow, ISubordinateWindow)}.Content(Contenido)`. El flujo es que después de `Window` va el bloque de los recursos declarados previamente en `Resources`, luego viene el de propiedades y `Content` es el último bloque, después de él no se escribe más nada. Una vez que coloques un miembro de los bloques, después de este solo puedes escribir miembros de el mismo bloque o del bloque que sigue, pero una vez que coloques un miembro del bloque siguiente, ya no podrás poner de boques anteriores.
- La API debe ser escrita de forma que el `IntelliSense` sea guía para el desarrollador, usando los contextos como fuente de datos, por este motivo hay una estructura de codificación que explica el flujo, esto es pensado para el programador novato, donde el propio `IntelliSense` le va guiando.
- La View puede ser un Button, un Label, un Select, etc. En la biblioteca se proporcionará algunos controles básicos para no tener que declarar `.View()`, pues es menos legible, pero va a estar ahí, para que los desarrolladores puedan crear sus propios controles. Estos controles tendrán una clasificación o mejor dicho agrupación por tipo de interacción. Entonces quedaría así:
  ```csharp
  .Content(c => c
      .Sapcing(8)
      .Children(ch => ch
          .ImmediateAction<IPushButton>(pb => pb
              .Name("BtnGuardar")
              .Content(c => c.Children(ch => ch.Text("Guardar Datos")))
          )
      )
  )
  ```
- Agrupación de controles: ImmediateAction, DataEntry, ExclusionarySelector, Navigation, BrowserData, PassiveInteraction, GroupContainers.

### Principio de Garantía de Completitud

- Todas las propiedades y parametros que forman partes del flujo, con la exepcion de los que se expongan, deben estar inicializados internamente o sea deben tener un valor asignado intenamente para respetar este principio.
- Cuando se escribe así `.Window(...)` es `Window<IWindow>(...)`, donde `IWindow` es el alias público de `IIndependentWindow`. Lo implícito es el tipo, no el concepto. Lo mismo aplica para `.AlternativeWindow(...)` y `.View(...)`..
- Cuando se escribe así `.Content(...)` es realmente es `.Content<IVerticalStackLayout>(...)` lo que pasa es que es implícito. Para poder establecer otro tipo de diseño, se debe usar `.Content<TStackLayout>(...)` o sea `IHorizontalStackLayout` o `IDockStackLayout`.
- Para `Window.{estilos, imágenes, servicios o sea todo lo que se necesite del Resources}.{Propiedades propias según el tipo de ventana (IIndependentWindow, IAssociatedWindow, ISubordinateWindow)}.Content(Contenido)`, Los recursos pueden ser omitidos, pues estos se heredan o no se usan, para las propiedades, estas se heredan o tienen inicialización interna.
- Cada uno de los grupos tendrán una interfaz y esta es su base, también tendrán una interfaz implícita que funcionaria como la opción más usada, por ejemplo, en vez de usar `.ImmediateAction<IPushButton>(...)`, usamos `.ImmediateAction(...)` donde `IPushButton` es implícito o predeterminado.

## Generación de Código en Tiempo de Compilación (Source Generators)

Para mantener la compatibilidad con `Native AOT` y ofrecer un rendimiento extremo, el framework **no usa Reflexión en tiempo de ejecución**.

En su lugar, actúa como un DSL (Domain Specific Language). Al ejecutar `dotnet build`, los Source Generators (Roslyn) analizan el Fluent API y generan:
1. **Código *boilerplate* de interoperabilidad Win32** (P/Invoke con `[LibraryImport]`).
2. **Alias fuertemente tipados** que conectan los `Name` del `Layout` con los manejadores del `Behavior`, eliminando *magic strings*.
3. **Puentes de suscripción MVU** (opcionales): solo si el desarrollador usa `State<T>` y `Signal`.

> NOTA: Los puntos (1) y (2) son obligatorios y agnósticos del patrón de estado. El punto (3) es el único ligado a MVU. Como `Behavior` expone el `HWND` y el `WndProc` directamente, los desarrolladores que prefieran otro paradigma pueden implementar sus propios manejadores sin necesidad de generadores adicionales. Por esta razón, en `EasyWindowsApplication.Generators` solo existe `MvuBindingEmitter.cs`.

## Gestión de Estado (Agnosticismo de Patrones)

El framework no impone un único paradigma de estado. Aunque adopta **MVU (Model-View-Update)** como camino principal por su coherencia natural con la Fluent API —ambos son unidireccionales y el estado fluye en una sola dirección, igual que la cadena de métodos—, el agnosticismo es real gracias al diseño de la sección `Behavior`.

`Behavior` expone directamente el `HWND` y el `WndProc` de Win32. Esto constituye una **válvula de escape universal**: cualquier paradigma (MVVM, MVC, Flux, o incluso llamadas directas a la API de Windows) puede implementarse sobre estas primitivas. El `WndProc` nativo es el "event bus" de más bajo nivel del sistema operativo; todo paradigma de interacción es, en última instancia, una abstracción sobre él.

**Por lo tanto, la jerarquía es:**

  - **MVU** → camino oficial, optimizado por los Source Generators con `MvuBindingEmitter.cs`.
  - **Win32 puro (`HWND` + `WndProc`)** → válvula de escape para cualquier otro patrón o necesidad de control total.

### Consecuencias arquitectónicas

- Las primitivas de estado `State<T>`, `Update` y `Signal` viven en `StateManagement/` y solo son relevantes para el camino MVU.
- Los módulos visuales exponen propiedades mutables y eventos crudos, sin acoplarse a ningún paradigma.
- Los **Source Generators** solo escriben el puente MVU. Cualquier otro patrón se implementa directamente en `Behavior` sin necesidad de generación de código adicional.
- Si el desarrollador usa `Behavior` con `HWND` y `WndProc`, el framework no interfiere: la arquitectura se pliega y deja el control en manos del programador.

### SETTINGS (Tercer grupo de Resources)

**Definición**: Un **Setting** es un valor de configuración con nombre que la aplicación
necesita para determinar su comportamiento en tiempo de ejecución. A diferencia de los
ASSETS (definen apariencia) y los SERVICIOS (definen lógica de negocio), los SETTINGS
definen *cómo se comporta la aplicación según el contexto*: idioma, tema, preferencias
del usuario, conexiones, etc.

**No necesita directorio nuevo.** Los valores por defecto de configuración se colocan
en `Resources/Raw/appsettings.json`. El framework lo detecta automáticamente por
convención de nombre y ubicación — igual que `AppIcon/appicon.svg` se convierte en
el ícono de la app sin declaración explícita en la API.

**Flujo en tiempo de ejecución:**

1. En startup, el framework busca el recurso embedido `Resources.Raw.Appsettings_Json`
   (en la sección .rsrc del ejecutable como RCDATA, cero I/O de disco). Si existe,
   esos son los valores por defecto.
2. Luego busca `appsettings.json` al lado del .exe (o en la ruta configurada en
   `.PersistencePath()`). Si existe, hace merge: los valores del disco tienen
   prioridad sobre los embedidos.
3. Durante la ejecución, las lecturas se sirven desde un diccionario en RAM.
4. Al guardar (`Save()` o `AutoSave`), solo escribe el archivo en disco — nunca
   modifica el embedido.

**API Fluent (Settings es un subgrupo opcional dentro de Resources):**

```csharp
WindowsApplication
    .Resources(r => r
        .Settings(s => s
            .PersistencePath("./appsettings.json")     // opcional: ruta en disco para merge+guardado
            .AutoSave(true)))                           // opcional: flush automático al hacer Set()
    .Layout(...)
    .Initialize();
```

**Si solo quieres defaults embedidos sin persistencia en disco, omite Settings por completo:**
```csharp
// appsettings.json embedido en Raw/ se carga automáticamente
WindowsApplication
    .Resources(r => r ...)
    .Layout(...)
    .Initialize();
```
**Proveedor custom (válvula de escape):**
- El framework incluye una implementación por defecto (JsonSettingsProvider, internal) que lee/escribe appsettings.json en disco.
- Si el desarrollador necesita otro medio de persistencia (Registro de Windows, base de datos, archivo encriptado), puede implementar la interfaz IAppSettingsProvider y registrarla como servicio:
```csharp
// Share/IAppSettingsProvider.cs
namespace EasyWindowsApplication;

public interface IAppSettingsProvider
{
    string? Get(string key);
    void Set(string key, string? value);
    void Load();
    void Save();
    event Action<string, string?>? SettingChanged;
}
WindowsApplication
    .Resources(r => r
        .Settings(s => s
            .PersistencePath("./appsettings.json")               // defaults embedidos + merge en disco
            .WithProvider<RegistrySettingsProvider>())            // ← reemplaza el proveedor por defecto
        .Services(s => s
            .Singleton<IAppSettingsProvider, RegistrySettingsProvider>()))
    .Layout(...)
    .Initialize();
```
> NOTA: IAppSettingsProvider solo debe implementarse si el developer necesita algo distinto a JSON en disco. El framework ya incluye una implementación interna para el 90% de los casos. Settings siempre es opcional, el developer puede ignorarlo y usar sus propios servicios de configuración via DI si prefiere otro enfoque.

### Plantillas (Visual Studio 2026)

El repo distribuye dos plantillas de proyecto, instalables con `dotnet new install EasyWindowsApplication.Templates` (o desde VS 2026: Crear nuevo proyecto → Easy Windows Application / Simple Easy Windows Application):

| Plantilla | Nombre corto | Qué genera |
|---|---|---|
| **Simple Easy Windows Application** | `simpleeasywinapp` | Un único fichero `Program.cs` (top-level statements). |
| **Easy Windows Application** | `easywinapp` | Secciones: `Program.cs` + `Resources.cs` + `Layout.cs` + `Behavior.cs`. Con `--Simple false` descompone el Layout en `Views/` y `Controls/` (experimental). |

#### `easywinapp` (por defecto, secciones)

**Scaffolding que genera el template:**

```text
MiApp/
├── MiApp.csproj
├── MiApp.ico                        ← icono regenerado en build desde Resources/AppIcon/appicon.svg
├── Program.cs                       ← WindowsApplication.Resources(...).Layout(...).Behavior(...).Initialize();
├── Resources.cs                     ← ResourcesConfig.ConfigureResources
├── Layout.cs                        ← LayoutConfig.ConfigureLayout
├── Behavior.cs                      ← BehaviorConfig.ConfigureBehavior
└── Resources/
    ├── AppIcon/appicon.svg          ← placeholder (logo por defecto)
    ├── Images/logo.png
    └── Raw/appsettings.json         ← defaults embedidos
```

`Layout.cs` contiene directivas `#if (Simple)` / `#else`:

- `--Simple true` (por defecto): Layout inline dentro de `LayoutConfig`.
- `--Simple false` (experimental): `LayoutConfig` delega en `Views/MainWindowView.cs`, `Views/MsgErrorWindowView.cs` y `Controls/CustomCtrl.cs`. Las Views/Custom control se excluyen en `template.json` cuando `Simple` es true.

El repo compila la variante compuesta (la más completa): así CI ejercita el generador y las Views sin necesidad de exclusiones en el csproj. En la variante compuesta el generador NO emite alias de ventanas (`bh.MainWindow`) — ver limitación en "Ideas sobre la arquitectura".

**Contenido del `.csproj` generado:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PublishAot>true</PublishAot>
    <AllowUnsafeBlocks>True</AllowUnsafeBlocks>
    <ApplicationIcon>MiApp.ico</ApplicationIcon>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\EasyWindowsApplication\EasyWindowsApplication.csproj" />
    <ProjectReference Include="..\..\EasyWindowsApplication.Generators\EasyWindowsApplication.Generators.csproj"
                      OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Resources\Raw\appsettings.json" />
  </ItemGroup>
</Project>
```

**Contenido de `Program.cs` generado:**

```csharp
using EasyWindowsApplication;
using MiApp;

WindowsApplication
    .Resources(ResourcesConfig.ConfigureResources)
    .Layout(LayoutConfig.ConfigureLayout)
    .Behavior(BehaviorConfig.ConfigureBehavior)
    .Initialize();
```

#### `simpleeasywinapp`

Un único fichero `Program.cs` (top-level statements) equivalente al ejemplo de la sección "Ideas sobre la arquitectura" (Template 1).

**Workloads/Seeds necesarios para el template:**

| Paquete | Rol |
|---|---|
| `EasyWindowsApplication` (NuGet) | Framework + `Share/Infrastructure` (`IconGenerator`, `.targets` de icono incluido) |
| `SkiaSharp` (transitivo) | Solo build-time (generación de .ico desde SVG), `PrivateAssets=all` |
| `EasyWindowsApplication.Templates` (NuGet) | Contiene los templates `dotnet new easywinapp` y `simpleeasywinapp` |

> NOTA: hoy las plantillas referencian el framework por `ProjectReference` local al repo. En el futuro, al publicar el paquete NuGet, la referencia pasará a `<PackageReference Include="EasyWindowsApplication" Version="*" />`.


## Ideas sobre la arquitectura de proyecto y diseño de framework en uso
1. Plantilla "simpleeasywinapp" (un solo fichero, top-level statements)
  - Program.cs
    ```csharp
    int counter = 0;

    WindowsApplication
        .Layout(ly => ly
            .Window(iw => iw
                .Name("MainWindow")
                .Title("Easy Win App")
                .Dimensions(420, 280)
                .Position(WindowPositionOnScreen.Center)
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

2. Plantilla "easywinapp" (secciones: Program + Resources + Layout + Behavior) — por defecto
  - Program.cs
    ```csharp
    WindowsApplication
        .Resources(ResourcesConfig.ConfigureResources)
        .Layout(LayoutConfig.ConfigureLayout)
        .Behavior(BehaviorConfig.ConfigureBehavior)
        .Initialize();
    ```

  - Resources.cs
    ```csharp
    using EasyWindowsApplication.CoreModule.Frontend;

    namespace <NombreApp>;

    public static class ResourcesConfig
    {
        public static void ConfigureResources(IResourcesDictionary res) =>
            res.Setting(sb => sb
                .UseWinApi()
            );
    }
    ```

  - Layout.cs
    ```csharp
    using EasyWindowsApplication.LayoutModule.Frontend;
    using EasyWindowsApplication.Win32ControlsModule.Frontend;
    using EasyWindowsApplication.WindowingModule.Frontend;

    namespace <NombreApp>;

    public static class LayoutConfig
    {
        public static void ConfigureLayout(ILayoutBuilder ly) =>
            ly.Window(iw => iw
                .Name("MainWindow")
                .Title("Easy Win App")
                .Dimensions(420, 280)
                .Position(WindowPositionOnScreen.Center)
                .Content(c => c
                    .Children(ch => ch
                        .View<IButton>(btn => btn
                            .Name("BtnIncrement")
                            .Text("Click me")
                        )
                    )
                )
            );
    }
    ```

  - Behavior.cs
    ```csharp
    using EasyWindowsApplication;
    using EasyWindowsApplication.CoreModule.Frontend;

    namespace <NombreApp>;

    public static class BehaviorConfig
    {
        private static int _counter;

        public static void ConfigureBehavior(IBehaviorBuilder bh) =>
            bh.BtnIncrement.OnClick(() =>
            {
                _counter++;
                bh.BtnIncrement.Text = $"Click: {_counter}";
            });
    }
    ```

3. Plantilla "easywinapp" con `--Simple false` (composición: Views/ y Controls/) — EXPERIMENTAL hasta el rework semántico del generador
  - Program.cs
    ```csharp
    WindowsApplication
        .Resources(ResourcesConfig.ConfigureResources)
        .Layout(LayoutConfig.ConfigureLayout)
        .Behavior(BehaviorConfig.ConfigureBehavior)
        .Initialize();
    ```

  - Resources.cs
    ```csharp
    using EasyWindowsApplication.CoreModule.Frontend;

    namespace <NombreApp>;

    public static class ResourcesConfig
    {
        public static void ConfigureResources(IResourcesDictionary res) =>
            res.Setting(sb => sb
                .UseWinApi()
            );
    }
    ```

  - Layout.cs
    ```csharp
    using <NombreApp>.Views;

    namespace <NombreApp>;

    public static class LayoutConfig
    {
        public static void ConfigureLayout(ILayoutBuilder ly) =>
            ly.Window(MainWindowView.MainWindowConfig)
              .AlternativeWindow(MsgErrorWindowView.MsgErrorWindowConfig);
    }
    ```

    > LIMITACIÓN ACTUAL (hasta rework de `ExtractNamedInfo` en `EasyBehaviorGenerator.cs`): los `Name` dentro de métodos extraídos (p. ej. `MainWindowView.MainWindowConfig`) NO generan alias de ventana. Por eso `Behavior.cs` de esta variante solo usa `bh.BtnIncrement` y no `bh.MainWindow`. En la variante simple (inline) el generador sí emite `bh.MainWindow`.

  - MainWindowView.cs
    ```csharp
    using <NombreApp>.Controls;

    namespace <NombreApp>.Views;

    public static class MainWindowView
    {
        public static void MainWindowConfig(IWindowConfig iw) =>
            iw.Name("MainWindow")
              .Title("Easy Win App")
              .Dimensions(420, 280)
              .Position(WindowPositionOnScreen.Center)
              .Content(c => c
                  .Padding(8)
                  .Children(ch => ch
                      .View<IButton>(btn => btn
                          .Name("BtnIncrement")
                          .Text("Click me")
                      )
                      .View<ILabel>(lb => lb
                          .Text("Secciones + Views")
                      )
                      .View(CustomCtrl.CustomControlConfig)
                  )
              );
    }
    ```

  - MsgErrorWindowView.cs
    ```csharp
    namespace <NombreApp>.Views;

    public static class MsgErrorWindowView
    {
        public static void MsgErrorWindowConfig(IWindowConfig iw) =>
            iw.Name("MsgErrorWindow")
              .Title("Error")
              .Dimensions(320, 180)
              .Content(c => c
                  .Padding(8)
                  .Children(ch => ch
                      .View<ILabel>(lb => lb
                          .Text("Algo ha fallado.")
                      )
                  )
              );
    }
    ```

  - CustomControl.cs
    ```csharp
    namespace <NombreApp>.Controls;

    public static class CustomCtrl
    {
        public static void CustomControlConfig(IViewBuilder cc) =>
            cc.Name("CustomCtrl")
              .Content(c => c
                  .Padding(8)
                  .Spacing(8)
                  .Children(ch => ch
                      .View<ILabel>(lb => lb
                          .Text("Contenido del control")
                      )
                  )
              );
    }
    ```

  - Behavior.cs
    ```csharp
    using EasyWindowsApplication;
    using EasyWindowsApplication.CoreModule.Frontend;

    namespace <NombreApp>;

    public static class BehaviorConfig
    {
        private static int _counter;

        public static void ConfigureBehavior(IBehaviorBuilder bh) =>
            bh.BtnIncrement.OnClick(() =>
            {
                _counter++;
                bh.BtnIncrement.Text = $"Click: {_counter}";
            });
    }
    ```

> **Regla de estado:** estado local del handler → campo `static` en `BehaviorConfig`; estado compartido entre handlers/ventanas → `Resources` (settings/DI).
