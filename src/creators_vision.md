## Proyecto

- Por el momento usaremos un solo proyecto para la biblioteca de clases.
- Para mejor rendimiento y futuros ajustes de rendimiento, la biblioteca se publica con `Native AOT`.
- Como consume a las bibliotecas nativas de `Win32` usara modo inseguro.
- Un proyecto CLI de C# para uso interno, con el objetivo de ver cada funcionalidad desarrollada o en desarrollo, este lo llamaremos `Sample`.
- `Common` es para código usado o compartido o repetitivo entre los módulos, este siempre debe ser interno (internal).
- `Share` es para código usado o compartido o repetitivo que se use internamente, pero también externamente (public).
- `WindowsApplication.cs` el punto de entrada del Fluent Api.

**Estructura del proyecto**
```
EasyWindowsApplication > [.gitignore, LICENSE, README.md, src/] 
src/ > [EasyWindowsApplication.slnx, EasyWindowsApplication/, Sample]
EasyWindowsApplication/ > [WindowsApplication.cs, Common/, Share/, ImmediateActionModule/, ]
```

- Cada funcionalidad va en un módulo y dentro de este se divide en dos directorios `Frontend` y `Backend` donde todo el código de frontend puede ser público o interno, pero el de backend, siempre va a ser interno.
- En cada directorio `Backend` y en `Common` contendrán estos ficheros `Entities.cs`, `Procedures.cs`, `Win32.cs`.
- `Entities.cs` contendrá todas las estructuras de datos que usaran en `Win32.cs` para comunicarse ya sea pasarlas como argumentos o para obtenerlas como resultados.
- `Win32.cs` todas las llamadas a Win32 necesarias para el módulo. 
- `Procedures.cs` contendrá todas las funciones que ajustadas para que el Frontend las use o sea es como el Core de la biblioteca.
- Si una función o estructura del `Backend` se repite en otro modulo, se debe pasar a `Common`.

## Layout

```csharp
.Window<IIndependentWindow>()
.Window<IAssociatedWindow>()
.Window<ISubordinateWindow>()

.Content<IVerticalStackLayout>(...)
.Content<IHorizontalStackLayout>(...)
.Content<IDockStackLayout>(...)

Layout(ly => ly
    .Window(iw =>  iw
        .Name("MainWindow")
        .Title("Mi App Ultrarrápida")
        .Dimensions(800, 600)
        .Content(c => c.Sapcing(8).Window<ISubordinateWindow>(...))
    )
    ly.Window<IAssociatedWindow>(aw => aw
        .Name("MsgError")
        .Dimensions(300, 200)
        .Content(c => c.Sapcing(8).Window<ISubordinateWindow>(...))
    )
)

.Content(c => c
    .Sapcing(8)
    .Window<ISubordinateWindow>(sw => sw
        .Name("BtnGuardar")
        .Content(c => c.Text("Guardar Datos"))
    )
)         
```

- `IIndependentWindow`, `IAssociatedWindow` y `ISubordinateWindow` heredan de `IBaseWindow`.
- `IVerticalStackLayout`, `IHorizontalStackLayout` y `IDockStackLayout` heredan de `IStackLayout`.
- Los `Layout` solo admiten un `IIndependentWindow` y múltiples `IAssociatedWindow`, estos deben ir después de `IIndependentWindow`. El flujo es que solo podrás escribir `Window` y después de este podrás poner todos los `Window<IAssociatedWindow>` que quieras, para ayudar al desarrollador y al compilador podremos usar `IAssociatedWindow`, quedando así:
```csharp
ly.AssociatedWindow(aw => aw
        .Name("NotificationMsg")
        .Dimensions(300, 200)
        .Content(c => c.Sapcing(8).Window<ISubordinateWindow>(...))
    );
ly.AssociatedWindow(aw => aw
        .Name("ReconnectMsg")
        .Dimensions(300, 200)
        .Content(c => c.Sapcing(8).Window<ISubordinateWindow>(...))
    )
```
- Para los `IBaseWindow` la estructura fluida es `Window.{estilos, imágenes, servicios o sea todo lo que se necesite del Resources}.{Propiedades propias según el tipo de ventana (IIndependentWindow, IAssociatedWindow, ISubordinateWindow)}.Content(Contenido)`. El flujo es que después de `Window` va el bloque de los recursos, luego viene el de propiedades y `Content` es el último bloque, después de él no se escribe más nada. Una vez que coloques un miembro de los bloques, después de este solo puedes escribir miembros de el mismo bloque o del bloque que sigue, pero una vez que coloques un miembro del bloque siguiente, ya no podrás poner de boques anteriores.
- La API debe ser escrita de forma que el `IntelliSense` sea guía para el desarrollador, usando los contextos como fuente de datos, por este motivo hay una estructura de codificación que explica el flujo, esto es pensado para el programador novato, donde el propio `IntelliSense` le va guiando.
- La Window<ISubordinateWindow> puede ser un Button, un Label, un Select, etc. En la biblioteca se proporcionará algunos controles básicos para no tener que declarar `.Window<ISubordinateWindow>()`, pues es menos legible, pero va a estar ahí, para que los desarrolladores puedan crear sus propios controles. Estos controles tendrán una clasificación o mejor dicho agrupación por tipo de interacción. Entonces quedaría así:
```csharp
.Content(c => c
    .Sapcing(8)
    .ImmediateAction<IPushButton>(pb => pb
        .Name("BtnGuardar")
        .Content(c => c.Text("Guardar Datos"))
    )
)     
```
- Agrupación de controles: ImmediateAction, DataEntry, ExclusionarySelector, Navigation, BrowserData, PassiveInteraction, GroupContainers.

### Principio de Garantía de Completitud
- Cuando se escribe así `.Window(...)` es `Window<IndependentWindow>(...)` lo que pasa es que es implícito.
- Cuando se escribe así `.Content(...)` es realmente es `.Content<IVerticalStackLayout>(...)` lo que pasa es que es implícito.
- Para `Window.{estilos, imágenes, servicios o sea todo lo que se necesite del Resources}.{Propiedades propias según el tipo de ventana (IIndependentWindow, IAssociatedWindow, ISubordinateWindow)}.Content(Contenido)`, Los recursos pueden ser omitidos, pues estos se heredan o no se usan, para las propiedades, estas se heredan o tienen inicialización interna.
- Cada uno de los grupos tendrán una interfaz y esta es su base, también tendrán una interfaz implícita que funcionaria como la opción más usada, por ejemplo, en vez de usar `.ImmediateAction<IPushButton>(...)`, usamos `.ImmediateAction(...)` donde `IPushButton` es implícito o predeterminado.
