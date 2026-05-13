vamos a organizar algunas cosas, empecemos por Layout, esto es lo que yo quiero:
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
- Cuando se escribe así `.Window(...)` es `Window<IndependentWindow>(...)` lo que pasa es que es implícito.
- Cuando se escribe así `.Content(...)` es realmente es `.Content<IVerticalStackLayout>(...)` lo que pasa es que es implícito.
- La API debe ser escrita de forma que el `IntelliSense` sea guía para el desarrollador, usando los contextos como fuente de datos, por este motivo hay una estructura de codificación que explica el flujo, esto es pensado para el programador novato, donde el propio `IntelliSense` le va guiando.
- Para los `IBaseWindow` la estructura fluida es `Window.{estilos, imágenes, servicios o sea todo lo que se necesite del Resources}.{Propiedades propias según el tipo de ventana (IIndependentWindow, IAssociatedWindow, ISubordinateWindow)}.Content(Contenido)`. El `Content` es el último bloque, después de él no se escribe más nada.
- Los `Layout` solo admiten un `IIndependentWindow` y múltiples `IAssociatedWindow`, estos deben ir después de IIndependentWindow.
- La Window<ISubordinateWindow> puede ser un Button, un Label, un Select, etc. En la biblioteca se proporcionará algunos controles básicos para no tener que declarar `.Window<ISubordinateWindow>()`, pues es menos legible, pero va a estar ahí, para que los desarrolladores puedan crear sus propios controles. Estos controles tendrán una clasificación o mejor dicho agrupación por tipo de interacción. Entonces quedaría así:
```csharp
.Content(c => c
    .Sapcing(8)
    .<ISubordinateWindow>(sw => sw
        .Name("BtnGuardar")
        .Content(c => c.Text("Guardar Datos"))
    )
)     
```
