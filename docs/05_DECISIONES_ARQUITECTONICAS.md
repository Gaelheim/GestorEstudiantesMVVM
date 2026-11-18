# 05. Decisiones Arquitectónicas (ADR - Architectural Decision Records)

## 1. Decisiones Adoptadas y Justificación Técnica

### Decisión 1: Selección del Target Framework `.NET 9.0` (`net9.0` / `net9.0-windows`)
- **Contexto:** Tras evaluar el fin de soporte de `.NET 8.0` y verificar los componentes instalados en el entorno local (**Visual Studio Community 2022 v17.14.37**, **SDK de .NET `9.0.316`** y **Runtime de escritorio `Microsoft.WindowsDesktop.App 9.0.18`**), se decidió ejecutar la solución con **`.NET 9.0`** (`net9.0` / `net9.0-windows`), dejando documentada en `08_GUIA_MIGRACION_NET10.md` la ruta de actualización futura a `.NET 10.0 LTS` cuando se instale Visual Studio 2026.
- **Decisión:** Configurar los proyectos de bibliotecas (`Domain`, `Application`, `Infrastructure`) en `net9.0` y el proyecto WPF (`Presentation`) en `net9.0-windows`.
- **Justificación técnica:** Aprovecha el SDK más moderno disponible en el entorno (`9.0.316`) con soporte nativo en Visual Studio 2022 `17.14.37` y C# 13. Elegir `net9.0` para las capas de dominio, aplicación e infraestructura (en lugar de `net9.0-windows`) garantiza a nivel de compilador que las tres capas internas sean independientes de plataforma y no puedan referenciar accidentalmente ensamblados exclusivos de WPF (`PresentationFramework`, `WindowsBase`).

### Decisión 2: Integración Unificada de MVVM + Clean + Onion + Hexagonal en 4 Proyectos
- **Contexto:** El objetivo académico exige demostrar el uso conjunto de cuatro enfoques arquitectónicos sin duplicar capas ni sobre-ingenierizar la solución.
- **Decisión:** Estructurar la solución en exactamente cuatro proyectos (`Domain`, `Application`, `Infrastructure`, `Presentation`).
- **Justificación técnica:**
  - **Onion y Clean Architecture** comparten la misma topología concéntrica (`Domain` -> `Application` -> `Infrastructure` / `Presentation`).
  - **Hexagonal Architecture** se materializa dentro de esa misma estructura ubicando los Puertos (`Interfaces/` y `UseCases/`) en `Application` y los Adaptadores en `Infrastructure` (secundarios) y `Presentation` (primarios).
  - **MVVM** se aplica exclusivamente dentro de `Presentation`.
  - De este modo, cada archivo tiene un único propósito claro pero satisface simultáneamente los principios de las cuatro arquitecturas.

### Decisión 3: Inyección de Dependencias con Paquetes Oficiales NuGet (`Microsoft.Extensions.DependencyInjection` `9.0.0`) en el Composition Root
- **Contexto:** Los `ViewModels` no deben acoplarse a implementaciones concretas de infraestructura y la solución debe ser 100% reproducible mediante gestión estándar de paquetes.
- **Decisión:** Utilizar los paquetes oficiales NuGet alineados con `.NET 9.0`: `Microsoft.Extensions.DependencyInjection.Abstractions` (`Version="9.0.0"`) en `GestorEstudiantes.Infrastructure` y `Microsoft.Extensions.DependencyInjection` (`Version="9.0.0"`) en `GestorEstudiantes.Presentation`, configurando el contenedor en `App.xaml.cs` (Composition Root).
- **Justificación técnica:** Reemplaza cualquier dependencia implícita a ensamblados compartidos del runtime (`FrameworkReference`) por paquetes NuGet oficiales y ligeros, evitando arrastrar referencias de ASP.NET Core hacia proyectos de escritorio o de infraestructura y asegurando compilaciones deterministas.

### Decisión 4: Persistencia Inicial en Memoria (`InMemoryEstudianteRepository`)
- **Contexto:** El proyecto necesita almacenar y consultar estudiantes sin depender inicialmente de un motor de base de datos externo.
- **Decisión:** Implementar el adaptador `InMemoryEstudianteRepository` en la capa `Infrastructure`.
- **Justificación técnica:** Permite ejecutar y demostrar la aplicación en cualquier computadora con Visual Studio sin requerir instalación ni configuración de SQL Server, a la vez que demuestra la potencia de **Hexagonal Architecture**: el almacenamiento en memoria se trata como un detalle técnico externo detrás del puerto `IEstudianteRepository`.

### Decisión 5: Uso de MVVM Puro sin Frameworks de Terceros Pesados
- **Contexto:** Existen librerías externas como Prism, Caliburn.Micro o CommunityToolkit.Mvvm.
- **Decisión:** Construir las bases del patrón MVVM (`ViewModelBase` con `INotifyPropertyChanged` y `RelayCommand` con `ICommand`) utilizando las interfaces nativas de .NET en las carpetas `ViewModels/` y `Commands/`.
- **Justificación técnica:** En un proyecto académico donde se evalúa la comprensión arquitectónica, implementar explícitamente `INotifyPropertyChanged` e `ICommand` permite explicar con total transparencia al profesor cómo funciona internamente el enlace de datos de WPF sin "magia oculta" ni generadores de código externos.

---

## 2. Ventajas y Limitaciones de la Arquitectura Adoptada

### Ventajas
1. **Alta Mantenibilidad y Bajo Acoplamiento:** Cualquier cambio en la interfaz gráfica XAML no afecta a los casos de uso ni al dominio.
2. **Sustituibilidad Total de la Persistencia (Plug & Play):** Pasar de memoria (`InMemoryEstudianteRepository`) a SQL Server, SQLite o un archivo JSON en el futuro solo requerirá crear un nuevo adaptador en `Infrastructure` y cambiar una línea en el contenedor de dependencias, con **cero cambios en `Domain`, `Application` y `ViewModels`**.
3. **Alta Testabilidad:** `Domain` y `Application` pueden probarse con pruebas unitarias automatizadas en milisegundos sin interfaz gráfica.
4. **Claridad Académica:** La separación física en cuatro proyectos `.csproj` hace evidente y demostrable la dirección de dependencias.

### Limitaciones
1. **Mayor Cantidad de Archivos Iniciales:** Requiere definir interfaces (puertos), DTOs y capas separadas en lugar de consultar datos directamente desde el *code-behind* de una ventana.
2. **Volatilidad del Almacenamiento en Memoria:** Al usar `InMemoryEstudianteRepository`, los datos registrados durante una sesión se reinician al cerrar la aplicación (comportamiento esperado y previsto para esta etapa).
3. **Mapeo entre Entidades y DTOs:** Exige transformar datos entre `Domain` y `Application` para proteger el encapsulamiento de las entidades.

---

## 3. Alternativas Descartadas

| Alternativa Descartada | Motivo del Descarte |
| :--- | :--- |
| **Código en Code-Behind (`MainWindow.xaml.cs`) sin MVVM** | Acopla la lógica directamente a los controles visuales de WPF, impide las pruebas unitarias y viola Clean Architecture y MVVM. |
| **Arquitectura Tradicional en 3 Capas (`UI -> BLL -> DAL`)** | En 3 capas clásicas, la capa de negocio (`BLL`) depende de la capa de acceso a datos (`DAL`). Esto viola Onion y Clean Architecture porque el negocio queda acoplado al almacenamiento. |
| **Crear un proyecto separado por cada una de las 4 arquitecturas (12+ proyectos)** | Generaría duplicación artificial e innecesaria de capas. Clean, Onion y Hexagonal son visiones complementarias de un mismo núcleo desacoplado. |
| **Base de Datos SQL Server desde la Fase 1** | Añade fricción de configuración de infraestructura y desvía el foco del objetivo principal de esta fase: establecer y validar la estructura arquitectónica limpia. |
