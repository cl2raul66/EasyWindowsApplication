# EasyWindowsApplication

**Un mini-framework declarativo, moderno y ultrarrápido para crear aplicaciones nativas de Windows en C#.**

`EasyWindowsApplication` trae el rendimiento puro y crudo del API nativa de Win32 a la era moderna de .NET. Mediante una **Fluent API** elegante y el poder de los **C# Source Generators** (Generación de código en tiempo de compilación), te permite construir interfaces gráficas de escritorio sin *boilerplate*, sin *magic strings* y con una separación de responsabilidades impecable.

Si crees que Win32 es demasiado complejo y que los frameworks modernos son demasiado pesados, estás en el lugar correcto.

## ✨ Características Principales

*   ⚡ **Rendimiento Nativo (Win32 Core):** Sin WebViews, sin motores de renderizado pesados. Por debajo, el framework se comunica directamente con `User32` y `GDI/DirectX`, garantizando un consumo de memoria mínimo y un rendimiento nativo.
*   🎨 **Fluent API Declarativa:** Define tus recursos, tu interfaz y tu lógica en un flujo continuo, legible y altamente cohesivo.
*   🧠 **Magia en Tiempo de Compilación (Source Generators):** Olvídate de buscar controles por su ID de texto. Al compilar (`dotnet build`), el framework lee tu diseño y genera alias fuertemente tipados para tus eventos. Si te equivocas, falla en el compilador, no en ejecución.
*   🏗️ **Jerarquía Espacial Moderna:** Redefinimos los arcaicos conceptos de Win32 (`WS_OVERLAPPED`, `WS_POPUP`, `WS_CHILD`) en un modelo mental moderno para el frontend:
    *   `IndependentWindow`: El contenedor principal (App Shell).
    *   `AssociatedWindow`: Capas superpuestas, modales y tooltips (Overlays).
    *   `SubordinateWindow`: Controles y elementos visuales confinados al flujo (Components).
*   📦 **Contenedor de Recursos Unificado:** Gestiona tus *Assets* (fuentes, imágenes) y tus *Servicios* (Inyección de Dependencias) en un solo lugar.

## 💻 Un vistazo al código

Crear una aplicación nativa nunca fue tan limpio. Todo se divide en 4 bloques lógicos: `Resources`, `Layout`, `Behavior` e `Initialize`.

```csharp
using EasyWindows;

WindowsApplication
    // 1. RECURSOS: Todo lo que la app necesita del exterior (Assets + DI)
    .Resources(rd => rd
        .Fonts(f => f.Add("Roboto", "Assets/Roboto.ttf"))
        .Images(i => i.Add("AppLogo", "Assets/logo.png"))
        .Services(s => s.AddSingleton<IMyDatabase, SqlDatabase>())
    )
    // 2. LAYOUT: El árbol visual de la aplicación
    .Layout(ly => ly
        .AddIndependentWindow("MainWindow", win => win
            .WithTitle("Mi App Ultrarrápida")
            .WithDimensions(800, 600)
            .AddSubordinateWindow("BtnGuardar", btn => btn
                .AsButton()
                .WithText("Guardar Datos")
            )
        )
    )
    // 3. BEHAVIOR: Lógica y enrutamiento de eventos fuertemente tipado
    // (¡Los alias como 'MainWindow' y 'BtnGuardar' son generados por Roslyn en tiempo real!)
    .Behavior(bh => bh
        .MainWindow.BtnGuardar.OnMouseClick(e => 
        {
            // Tu lógica aquí. Compatible con Eventos clásicos, MVVM o MVU.
            Console.WriteLine("¡Botón clickeado con rendimiento Win32!");
        })
    )
    // 4. INICIALIZACIÓN: Arranca el bucle de mensajes nativo
    .Initialize();
```

## 🛠️ ¿Cómo funciona por debajo?

A diferencia de los frameworks tradicionales que usan reflexión masiva en tiempo de ejecución, `EasyWindowsApplication` actúa como un **DSL (Domain Specific Language)**. 

Cuando escribes tu `Layout` y ejecutas `dotnet build`, nuestros Source Generators analizan tu árbol visual y escriben el código de interoperabilidad Win32 (P/Invoke), el registro de clases (`RegisterClassEx`) y el bucle de mensajes (`GetMessage`) por ti. El resultado es un binario tan rápido como si lo hubieras escrito en C++, pero con la belleza y seguridad de C# moderno.

## 🚀 Próximos Pasos
*   [Guía de Inicio Rápido (Quickstart)](#)
*   [Entendiendo la Arquitectura: Independent, Associated y Subordinate](#)
*   [Cómo usar el contenedor de Servicios (Dependency Injection)](#)

---
*Diseñado para el futuro del desarrollo de escritorio en Windows.*

---

### ¿Qué te parece?
Esta descripción ataca directamente los "puntos de dolor" de los desarrolladores (rendimiento, código espagueti, magic strings) y posiciona tu proyecto no solo como una biblioteca más, sino como una **herramienta de ingeniería avanzada** gracias a los Source Generators.
