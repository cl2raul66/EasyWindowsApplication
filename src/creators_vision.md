## Proyecto

- Por el momento usaremos un solo proyecto para la biblioteca de clases.
- Para mejor rendimiento y futuros ajustes de rendimiento, la biblioteca se publica con `Native AOT`.
- En el pasado, en `Win32.cs` usarías `[DllImport("user32.dll")]`. **En Native AOT esto es un antipatrón** porque requiere generación de código en tiempo de ejecución (JIT). Debes usar el atributo `[LibraryImport]`. Esto obliga a que tus métodos en `Win32.cs` sean `partial`.
- Como consume a las bibliotecas nativas de `Win32` usara modo inseguro.
- Un proyecto CLI de C# para uso interno, con el objetivo de ver cada funcionalidad desarrollada o en desarrollo, este lo llamaremos `Sample`.
- `Common` es para código usado o compartido o repetitivo entre los módulos, este siempre debe ser interno (internal).
- `Share` es para código usado o compartido o repetitivo que se use internamente, pero también externamente (public).
- `WindowsApplication.cs` el punto de entrada del Fluent Api.

**Estructura del proyecto**
```
EasyWindowsApplication/
│
├── .gitignore
├── LICENSE
├── README.md
└── src/
    ├── EasyWindowsApplication.slnx
    │
    ├── EasyWindowsApplication.Generators/
    │
    ├──  EasyWindowsApplication/
    │       ├── WindowsApplication.cs
    │       │
    │       ├── Share/ 
    │       │   └── IBaseWindow.cs
    │       │
    │       ├── Common/ 
    │       │   ├── Constants.cs 
    │       │   ├── Enums.cs
    │       │   ├── Entities.cs  
    │       │   ├── Win32.cs  
    │       │   └── Procedures.cs
    │       │
    │       ├── DataEntry/ 
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
    │       ├── ExclusionarySelector/ 
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
    │       ├── Navigation/ 
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
    │       ├── BrowserData/ 
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
    │       ├── PassiveInteraction/ 
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
    │       ├── GroupContainers/ 
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
    │           ├── Frontend/
    │           │   ├── IPushButton.cs
    │           │   ├── ISplitButton.cs
    │           │   ├── IHyperlinkButton.cs
    │           │   └── IImmediateAction.cs
    │           │
    │           └── Backend/
    │               ├── Constants.cs 
    │               ├── Enums.cs 
    │               ├── Entities.cs
    │               ├── Win32.cs 
    │               └── Procedures.cs
    │
    └── Sample/
```

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
- En `Frontend`, puede existir `Enums.cs` y `Dtos.cs` como parte de la interfaz pública, estos tambien estaran en `Share.cs` si se repiten en otros modulos.
- En `Common`, puede existir `Helpers.cs` con funciones de ayuda para el desarrollo, como por ejemplo, funciones de extensión para facilitar la lectura o escritura de código.

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
