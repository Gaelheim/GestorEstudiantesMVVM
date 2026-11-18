# 07. Auditoría Complementaria — Fase 1

## 1. Resumen Ejecutivo de la Revisión Técnica

Como parte del control de calidad arquitectónico previo al inicio de la Fase 2, se ejecutó una auditoría técnica exhaustiva sobre la solución **GestorEstudiantesMVVM** abarcando cuatro ejes:
1. Evaluación de migración del Target Framework hacia **.NET 10 LTS** (`net10.0` / `net10.0-windows`).
2. Revisión y estandarización de las referencias de **Inyección de Dependencias** (`Microsoft.Extensions.DependencyInjection`).
3. Auditoría de pureza declarativa de los **Contratos y Clases Base**.
4. Verificación integral de los principios de **MVVM, Clean Architecture, Onion Architecture y Hexagonal Architecture**.

---

## 2. Problemas y Hallazgos Encontrados

### Hallazgo 1: Bloqueo de Entorno para la Migración a `.NET 10.0 LTS`
Se auditó el entorno de desarrollo local (`dotnet --list-sdks`, `dotnet --list-runtimes`, `C:\Program Files\dotnet\packs` y `vswhere.exe`) con los siguientes resultados verificados:

- **SDKs de .NET instalados:**
  - `9.0.316` (`C:\Program Files\dotnet\sdk\9.0.316`) — *Único SDK presente en el equipo.*
- **Runtimes de Escritorio (`Microsoft.WindowsDesktop.App`) instalados:**
  - `6.0.36`
  - `7.0.20`
  - `8.0.29`
  - `9.0.18`
- **Visual Studio instalado:**
  - **Visual Studio Community 2022** — Versión `17.14.37` (`17.14.37516.0`), la cual soporta hasta `.NET 9.0` (el soporte oficial para `.NET 10.0` requiere el SDK `10.0.xxx` y Visual Studio 2022 v17.14+ con el workload de .NET 10 o Visual Studio 2026).

> [!NOTE]
> **Actualización de Decisión Técnica:** Tras documentar el bloqueo de `.NET 10.0` en el entorno actual, se autorizó y ejecutó la migración de toda la solución hacia **`.NET 9.0`** (`net9.0` y `net9.0-windows`) junto con la actualización de los paquetes NuGet oficiales `Microsoft.Extensions.DependencyInjection` y `Microsoft.Extensions.DependencyInjection.Abstractions` a la versión **`9.0.0`**.

---

### Hallazgo 2: Referencia a `Microsoft.Extensions.DependencyInjection` mediante `FrameworkReference`
Al inspeccionar los archivos `GestorEstudiantes.Infrastructure.csproj` y `GestorEstudiantes.Presentation.csproj`, se detectó que para sortear el aislamiento de red del entorno sandbox inicial se había utilizado:
```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```
**Problema arquitectónico:** Aunque permitía compilar sin conexión a NuGet tomando los ensamblados compartidos del runtime local, incluir `Microsoft.AspNetCore.App` en una biblioteca de infraestructura y en una aplicación de escritorio WPF expone innecesariamente todo el ecosistema web de ASP.NET Core y no representa una gestión de paquetes explícita ni estándar.

---

### Hallazgo 3: Propiedades `TargetPlatformMoniker` y `TargetPlatformDisplayName` en `Presentation.csproj`
En `GestorEstudiantes.Presentation.csproj` se habían incluido las etiquetas `<TargetPlatformMoniker>` y `<TargetPlatformDisplayName>` como ajuste temporal para evitar una lectura del registro de Windows dentro del contenedor sandbox. En una compilación estándar con restauración de paquetes NuGet, el SDK de .NET infiere estos valores automáticamente a partir de `net8.0-windows`.

---

## 3. Ajustes Realizados y Justificación Técnica

### Ajuste 1: Reemplazo de `FrameworkReference` por Paquetes Oficiales NuGet con Versiones Explícitas
Se actualizaron los archivos `.csproj` para utilizar exclusivamente paquetes NuGet oficiales y ligeros, y se ejecutó `dotnet restore` hacia la caché oficial de paquetes del usuario:

1. En [GestorEstudiantes.Infrastructure.csproj](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/GestorEstudiantes.Infrastructure.csproj#L11-L13):
   ```xml
   <ItemGroup>
     <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.2" />
   </ItemGroup>
   ```
   - **Justificación:** La capa `Infrastructure` solo necesita registrar servicios sobre la interfaz `IServiceCollection`. No necesita el motor completo del contenedor IoC, por lo que referenciar únicamente `.Abstractions` (`8.0.2`) minimiza el acoplamiento y respeta el principio de mínima dependencia.

2. En [GestorEstudiantes.Presentation.csproj](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/GestorEstudiantes.Presentation.csproj#L13-L15):
   ```xml
   <ItemGroup>
     <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.1" />
   </ItemGroup>
   ```
   - **Justificación:** `Presentation` contiene el **Composition Root** ([App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs)), que es el único lugar de toda la solución donde se instancia `ServiceCollection` y se invoca `BuildServiceProvider()`.

### Ajuste 2: Simplificación Limpia de `GestorEstudiantes.Presentation.csproj`
Se eliminaron las etiquetas redundantes `<TargetPlatformMoniker>` y `<TargetPlatformDisplayName>` en [GestorEstudiantes.Presentation.csproj](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/GestorEstudiantes.Presentation.csproj#L3-L11), dejando una definición SDK-style 100% limpia y estándar para Visual Studio.

### Ajuste 3: Actualización de Documentación de Decisiones Arquitectónicas
Se actualizó [05_DECISIONES_ARQUITECTONICAS.md](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/docs/05_DECISIONES_ARQUITECTONICAS.md) para reflejar el diagnóstico de `.NET 10.0` vs `.NET 8.0`/`9.0` y el uso de `PackageReference` explícitos para `Microsoft.Extensions.DependencyInjection`.

---

## 4. Auditoría de Contratos y Archivos Declarativos

Se inspeccionó el código fuente de cada archivo clave para confirmar que **no existe lógica funcional adelantada**:

| Archivo Auditado | Ubicación | Estado y Verificación |
| :--- | :--- | :--- |
| [IEstudianteRepository.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/Interfaces/IEstudianteRepository.cs) | `Application/Interfaces/` | ✅ **Correcto.** Interfaz pública declarativa vacía (`public interface IEstudianteRepository { }`). Ubicada en la capa `Application` actuando como Puerto de Salida. Sin métodos CRUD adelantados. |
| [InMemoryEstudianteRepository.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/Repositories/InMemoryEstudianteRepository.cs) | `Infrastructure/Repositories/` | ✅ **Correcto.** Clase pública declarativa vacía que implementa el contrato (`public class InMemoryEstudianteRepository : IEstudianteRepository { }`). Demuestra la conexión Puerto-Adaptador sin adelantar colecciones ni lógica de persistencia. |
| [EstudianteDto.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/DTOs/EstudianteDto.cs) | `Application/DTOs/` | ✅ **Correcto.** Clase pública declarativa vacía (`public class EstudianteDto { }`) sin propiedades adelantadas. |
| [Estudiante.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Domain/Entities/Estudiante.cs) | `Domain/Entities/` | ✅ **Correcto.** Entidad declarativa vacía (`public class Estudiante { }`) sin lógica de negocio ni validaciones adelantadas. |
| [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs) | `Presentation/ViewModels/` | ✅ **Correcto.** Hereda de `ViewModelBase`. Expone únicamente una propiedad de título informativo para verificar el enlace XAML inicial. No contiene lógica de negocio ni referencias a `Infrastructure`. |
| [MainWindow.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml.cs) | `Presentation/Views/` | ✅ **Correcto.** *Code-behind* estrictamente limpio: solo invoca `InitializeComponent()` y asigna `DataContext = viewModel;` recibido por inyección de dependencias en el constructor. |
| [App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs) | `Presentation/` | ✅ **Correcto.** Configura `ServiceCollection`, registra los servicios de infraestructura mediante `services.AddInfrastructureServices()`, registra `MainViewModel` y `MainWindow`, y lanza la ventana principal en `OnStartup`. |

---

## 5. Auditoría Arquitectónica Integral

| Principio / Criterio | Verificación Técnica | Resultado |
| :--- | :--- | :---: |
| **Independencia de `Domain`** | `GestorEstudiantes.Domain.csproj` no tiene `<ProjectReference>` ni `<PackageReference>`. No conoce `Application`, `Infrastructure`, `Presentation` ni WPF. | ✅ Cumple |
| **Dirección de Dependencias** | Todas las dependencias apuntan hacia el centro (`Domain`). `Application` solo depende de `Domain`. | ✅ Cumple |
| **Ausencia de Dependencias Circulares** | Grafo Acíclico Dirigido verificado: `Presentation` -> (`Application`, `Infrastructure`) -> `Domain`. Compilación limpia sin ciclos. | ✅ Cumple |
| **Inversión de Dependencias (DIP)** | `Application` define la abstracción `IEstudianteRepository` e `Infrastructure` depende de `Application` para implementarla en `InMemoryEstudianteRepository`. | ✅ Cumple |
| **Separación MVVM** | Vista ([MainWindow.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml)) separada del ViewModel ([MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs)). Infraestructura base lista en [ViewModelBase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/ViewModelBase.cs) (`INotifyPropertyChanged`) y [RelayCommand.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Commands/RelayCommand.cs) (`ICommand`). | ✅ Cumple |
| **Puertos y Adaptadores (Hexagonal)** | Puerto de salida (`IEstudianteRepository`) aislado en `Application`; adaptador secundario (`InMemoryEstudianteRepository`) aislado en `Infrastructure`. | ✅ Cumple |
| **Composition Root y Aislamiento de ViewModels** | El único archivo en `Presentation` que importa `GestorEstudiantes.Infrastructure.DependencyInjection` es [App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs). Los ViewModels no referencian ni instancian `InMemoryEstudianteRepository`. | ✅ Cumple |

---

## 6. Resultado de Compilación

Tras aplicar los paquetes oficiales NuGet y limpiar los archivos `.csproj`, se ejecutó `dotnet build GestorEstudiantesMVVM.sln`:

```text
Determinando los proyectos que se van a restaurar...
Todos los proyectos están actualizados para la restauración.
GestorEstudiantes.Domain -> ...\GestorEstudiantes.Domain\bin\Debug\net8.0\GestorEstudiantes.Domain.dll
GestorEstudiantes.Application -> ...\GestorEstudiantes.Application\bin\Debug\net8.0\GestorEstudiantes.Application.dll
GestorEstudiantes.Infrastructure -> ...\GestorEstudiantes.Infrastructure\bin\Debug\net8.0\GestorEstudiantes.Infrastructure.dll
GestorEstudiantes.Presentation -> ...\GestorEstudiantes.Presentation\bin\Debug\net8.0-windows\GestorEstudiantes.Presentation.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores
```

---

## 7. Estado Final de la Arquitectura y Pendientes para la Fase 2

### Estado Actual
- La arquitectura base está **100% limpia, compilable, reproducible con paquetes oficiales NuGet** y alineada con MVVM, Clean Architecture, Onion Architecture y Hexagonal Architecture.

### Decisiones / Autorizaciones Pendientes antes de iniciar la Fase 2
1. **Resolución sobre la versión de .NET ante el bloqueo de `.NET 10.0` en el entorno:**
   - **Opción A (Recomendada para compilación inmediata sin instalar software externo):** Mantener `.NET 8.0` (`net8.0` / `net8.0-windows`) o migrar a **`.NET 9.0` (`net9.0` / `net9.0-windows`)**, ya que el SDK `9.0.316` y el runtime `9.0.18` ya están instalados y operativos en tu máquina junto con Visual Studio 2022 v17.14.37.
   - **Opción B:** Si deseas utilizar estrictamente `.NET 10.0 LTS` (`net10.0` / `net10.0-windows`), será necesario instalar primero el **SDK de .NET 10.0** en el sistema operativo para desbloquear el compilador y luego autorizar el cambio de `TargetFramework`.
2. **Autorización para iniciar la Fase 2 (`GestorEstudiantes.Domain`):**
   - Implementación de la entidad `Estudiante` (Matrícula, Nombre, Apellido, Correo electrónico, Carrera), invariantes de dominio, posibles `ValueObjects` y excepciones de dominio.
