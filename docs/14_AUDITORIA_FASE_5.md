# 14. Auditoría Final — Fase 5 (`GestorEstudiantes.Presentation`)

## 1. Archivos Inspeccionados

Se realizó una auditoría técnica exhaustiva y de solo lectura sobre los siguientes archivos reales de la solución:

1. [MainWindow.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml)
2. [MainWindow.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml.cs)
3. [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs)
4. [RelayCommand.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Commands/RelayCommand.cs)
5. [App.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml)
6. [App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs)
7. [GestorEstudiantes.Presentation.csproj](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/GestorEstudiantes.Presentation.csproj)
8. [InfrastructureServiceRegistration.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/DependencyInjection/InfrastructureServiceRegistration.cs)

---

## 2. Evaluación de MVVM

| Criterio | Estado | Análisis Técnico |
| :--- | :---: | :--- |
| **Implementación de `INotifyPropertyChanged`** | **SIN OBSERVACIONES** | `MainViewModel` implementa directamente `INotifyPropertyChanged` ([MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs#L18-L42)) y utiliza el método privado `OnPropertyChanged([CallerMemberName] string? propertyName = null)` en las líneas 371–374. |
| **Notificación en propiedades** | **SIN OBSERVACIONES** | Todas las propiedades de estado (`Matricula`, `Nombre`, `Apellido`, `CorreoElectronico`, `Carrera`, `TextoBusqueda`, `Mensaje`, `EsError`, `EsEdicion`, `EstudianteSeleccionado`) comprueban si el valor cambió (`if (_campo != value)`) y disparan `OnPropertyChanged()`. Además, `EsEdicion` notifica sus propiedades calculadas dependientes (`MatriculaHabilitada` y `TituloFormulario`). |
| **Pureza del code-behind (`MainWindow.xaml.cs`)** | **SIN OBSERVACIONES** | [MainWindow.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml.cs#L10-L17) tiene únicamente 17 líneas: recibe `MainViewModel viewModel` por constructor, llama a `InitializeComponent()` y asigna `DataContext = viewModel;`. Cero lógica de negocio ni manejadores de eventos (`Click`, `SelectionChanged`). |
| **Simplicidad estructural** | **MENOR** | `MainViewModel` recibe por constructor `BuscarEstudianteUseCase` y lo guarda en `_buscarEstudianteUseCase`, pero no lo invoca porque el filtrado de la tabla se realiza en memoria de la vista mediante `ICollectionView.Filter`. *(Ver detalle en la sección 7 y 9)*. |

---

## 3. Evaluación de Data Binding (`MainWindow.xaml`)

| Elemento Visual | Binding Configurado en XAML | Clasificación |
| :--- | :--- | :---: |
| **Campo Búsqueda** | `Text="{Binding TextoBusqueda, UpdateSourceTrigger=PropertyChanged}"` (línea 135) | **SIN OBSERVACIONES** |
| **Tabla `DataGrid`** | `ItemsSource="{Binding EstudiantesView}"` y `SelectedItem="{Binding EstudianteSeleccionado, Mode=TwoWay}"` (líneas 153–154) | **SIN OBSERVACIONES** |
| **Columnas del `DataGrid`** | 5 `DataGridTextColumn` enlazadas a `Matricula`, `Nombre`, `Apellido`, `CorreoElectronico` y `Carrera` con `AutoGenerateColumns="False"` y `IsReadOnly="True"` (líneas 170–185) | **SIN OBSERVACIONES** |
| **Campo Matrícula** | `Text="{Binding Matricula, UpdateSourceTrigger=PropertyChanged}"` e `IsEnabled="{Binding MatriculaHabilitada}"` (líneas 223–224) | **SIN OBSERVACIONES** |
| **Campos Nombre, Apellido, Correo, Carrera** | `Text="{Binding ..., UpdateSourceTrigger=PropertyChanged}"` (líneas 231, 238, 245, 252) | **SIN OBSERVACIONES** |
| **Botones Guardar, Limpiar, Eliminar, Limpiar búsqueda** | Enlazados a `GuardarCommand`, `LimpiarCommand`, `EliminarCommand` y `LimpiarBusquedaCommand` (líneas 141, 262, 267, 272) | **SIN OBSERVACIONES** |
| **Panel de Mensajes y Estado de Error** | `Text="{Binding Mensaje}"` con `DataTrigger Binding="{Binding EsError}" Value="True"` que cambia fondo (`#EBF5FF` ➔ `#FEF2F2`), borde (`#93C5FD` ➔ `#FCA5A5`) y texto (`#0A1D42` ➔ `#991B1B`) (líneas 278–310) | **SIN OBSERVACIONES** |

---

## 4. Evaluación de `RelayCommand` y Comandos del ViewModel

Se inspeccionó [RelayCommand.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Commands/RelayCommand.cs#L8-L32) y su uso en [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs#L64-L67):

1. **Implementación de `ICommand` (`RelayCommand.cs`):**
   - Implementa `CanExecute(object? parameter)`, `Execute(object? parameter)`, el evento `CanExecuteChanged` y el método público `RaiseCanExecuteChanged()`.
   - Valida en el constructor que `execute` no sea `null` (`ArgumentNullException`).
   - **Clasificación:** **SIN OBSERVACIONES**.
2. **Evaluación de los 4 comandos en `MainViewModel`:**
   - **`GuardarCommand`:** Siempre habilitado (`_canExecute == null`), permitiendo que al pulsarlo actúen las validaciones de `Domain` y `Application` y se muestre el mensaje explicativo en la barra inferior si falta algún campo.
   - **`LimpiarCommand`:** Siempre habilitado para restablecer el formulario en cualquier momento.
   - **`EliminarCommand`:** Configurado con `_ => EsEdicion && EstudianteSeleccionado != null` (línea 66). Permanece **deshabilitado** cuando no hay estudiante seleccionado o cuando se está en modo registro.
   - **`LimpiarBusquedaCommand`:** Limpia `TextoBusqueda = string.Empty`.
3. **Actualización de disponibilidad (`RaiseCanExecuteChanged`):**
   - `_eliminarCommand.RaiseCanExecuteChanged()` se invoca cuando cambia `EsEdicion` (línea 188), cuando cambia `EstudianteSeleccionado` (línea 220) y al limpiar el formulario en `LimpiarFormularioInterno()` (línea 343).
   - **Clasificación:** **SIN OBSERVACIONES**.

---

## 5. Evaluación de `ObservableCollection` e `ICollectionView`

Se inspeccionó el manejo de colecciones en [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs#L44-L63) y las líneas 137–149, 230–239 y 346–363:

1. **Separación entre colección original y vista filtrada:**
   - `Estudiantes` (`ObservableCollection<EstudianteDto>`) contiene todos los elementos traídos desde `_obtenerEstudiantesUseCase.Ejecutar()`.
   - `EstudiantesView` (`ICollectionView`, obtenida mediante `CollectionViewSource.GetDefaultView(Estudiantes)`) aplica el predicado `FiltrarEstudiante` **sin eliminar ni alterar los elementos de `Estudiantes`**.
2. **Comportamiento del filtro (`FiltrarEstudiante`):**
   - Cuando `TextoBusqueda` está vacío o contiene solo espacios (`string.IsNullOrWhiteSpace(TextoBusqueda)`), devuelve `true` mostrando todos los estudiantes.
   - Cuando hay texto, aplica `.Trim()` y comprueba coincidencias parciales con `.Contains(criterio, StringComparison.OrdinalIgnoreCase)` sobre `Matricula`, `Nombre` y `Apellido`.
   - Cada vez que cambia `TextoBusqueda` (línea 146), se invoca `EstudiantesView.Refresh()`, actualizando el `DataGrid` instantáneamente.
3. **Sincronización tras Registro, Actualización y Eliminación:**
   - Después de registrar, actualizar o eliminar, `MainViewModel` llama a `CargarEstudiantes()`, el cual vacía `Estudiantes.Clear()`, vuelve a poblarla con la lista fresca de `_obtenerEstudiantesUseCase.Ejecutar()` y ejecuta `EstudiantesView.Refresh()`.
   - Esto garantiza que **actualizar un estudiante jamás introduce registros duplicados** en la tabla y que la vista siempre refleja el estado exacto del repositorio en memoria.
   - **Clasificación:** **SIN OBSERVACIONES**.

---

## 6. Evaluación del CRUD en `MainViewModel`

- **Registro (`Guardar` cuando `!EsEdicion`, líneas 245–257):**
  - Invoca `_registrarEstudianteUseCase.Ejecutar(Matricula, Nombre, Apellido, CorreoElectronico, Carrera)`.
  - Si la matrícula está duplicada, `RegistrarEstudianteUseCase` lanza `InvalidOperationException`, que es capturada en las líneas 284–287 mostrando el mensaje de error en rojo (`EsError = true`) sin limpiar el formulario para que el usuario pueda corregirlo.
  - Si los datos son inválidos, `Estudiante.Create` lanza `ValidacionEstudianteException` (capturada como `DomainException` en las líneas 280–283).
  - Si tiene éxito, recarga la tabla, limpia el formulario y muestra confirmación.
  - **Clasificación:** **SIN OBSERVACIONES**.
- **Búsqueda (`FiltrarEstudiante`, líneas 346–363):**
  - Filtra por matrícula, nombre y apellido sin mutar los datos almacenados.
  - **Clasificación:** **SIN OBSERVACIONES**.
- **Actualización (`Guardar` cuando `EsEdicion`, líneas 258–278):**
  - Toma la matrícula inmutable directamente de `var matriculaOriginal = EstudianteSeleccionado.Matricula;` (línea 266) y además el `TextBox` de matrícula está deshabilitado (`MatriculaHabilitada => !EsEdicion`).
  - Invoca `_actualizarEstudianteUseCase.Ejecutar(matriculaOriginal, Nombre, Apellido, CorreoElectronico, Carrera)`.
  - Actualiza la tabla sin duplicar registros, limpia el formulario y sale del modo edición.
  - **Clasificación:** **SIN OBSERVACIONES**.
- **Eliminación (`Eliminar`, líneas 294–323):**
  - Verifica `if (!EsEdicion || EstudianteSeleccionado == null)`.
  - Toma exclusivamente `var matriculaAEliminar = EstudianteSeleccionado.Matricula;` (línea 304), impidiendo que se elimine un estudiante distinto al seleccionado.
  - Invoca `_eliminarEstudianteUseCase.Ejecutar(matriculaAEliminar)`, recarga la tabla, limpia el formulario y muestra confirmación.
  - **Clasificación:** **SIN OBSERVACIONES**.
- **Limpieza (`Limpiar` y `LimpiarFormularioInterno`, líneas 325–344):**
  - Pone `_estudianteSeleccionado = null` y notifica `OnPropertyChanged(nameof(EstudianteSeleccionado))`.
  - Vacía `Matricula`, `Nombre`, `Apellido`, `CorreoElectronico` y `Carrera`.
  - Restablece `EsEdicion = false` (lo que automáticamente vuelve a poner `MatriculaHabilitada = true` y actualiza `TituloFormulario`).
  - Actualiza el estado de `_eliminarCommand`.
  - **Clasificación:** **SIN OBSERVACIONES**.

---

## 7. Evaluación de Inyección de Dependencias y Composition Root

Se inspeccionaron [App.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml), [App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs) e [InfrastructureServiceRegistration.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/DependencyInjection/InfrastructureServiceRegistration.cs):

1. **Uso de `InfrastructureServiceRegistration` e instancia única del repositorio:**
   - `App.xaml.cs` invoca `services.AddInfrastructureServices()` (línea 27), el cual registra `services.AddSingleton<IEstudianteRepository, InMemoryEstudianteRepository>()`.
   - Al ser `Singleton`, el contenedor IoC crea **una única instancia** de `InMemoryEstudianteRepository` durante toda la ejecución de la aplicación y la comparte entre todos los casos de uso. No existe ninguna segunda instancia accidental.
2. **Registro e inyección por constructor:**
   - Los 5 casos de uso, `MainViewModel` y `MainWindow` están registrados en `ConfigureServices` y reciben sus dependencias exclusivamente por constructor.
3. **Ausencia de mecanismos de inicio duplicados:**
   - [App.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml) **no tiene `StartupUri`**, por lo que WPF no intenta instanciar `MainWindow` con un constructor vacío; la ventana se resuelve y muestra únicamente en `App.OnStartup` mediante `ServiceProvider.GetRequiredService<MainWindow>()`.
4. **Aislamiento de `MainViewModel` respecto a `Infrastructure`:**
   - `MainViewModel.cs` no contiene ningún `using GestorEstudiantes.Infrastructure;` ni referencias al repositorio concreto.
5. **Análisis de `BuscarEstudianteUseCase` en `MainViewModel`:**
   - **Hallazgo (`MENOR`):** `BuscarEstudianteUseCase` está registrado en `App.xaml.cs` e inyectado en el constructor de `MainViewModel` (almacenado en el campo privado `_buscarEstudianteUseCase`, línea 22 y línea 57), pero **no se invoca en ningún método de `MainViewModel`** porque la búsqueda interactiva en la interfaz se resolvió mediante `ICollectionView` (`EstudiantesView.Filter = FiltrarEstudiante`).
   - **Evaluación:** No produce ningún error de compilación, ni advertencia del compilador, ni fallo funcional. Sin embargo, al no utilizarse dentro de `MainViewModel`, su inyección en el constructor de `MainViewModel` es prescindible (o alternativamente, si se elimina del constructor de `MainViewModel`, se simplifica aún más el ViewModel dejando solo los 4 casos de uso que sí invoca directamente: `Registrar`, `Obtener`, `Actualizar` y `Eliminar`).

---

## 8. Evaluación de Integridad Arquitectónica

| Verificación Arquitectónica | Estado | Clasificación |
| :--- | :--- | :---: |
| **`Domain` sin referencias externas** | `GestorEstudiantes.Domain.csproj` no tiene referencias a proyectos ni paquetes NuGet. | **SIN OBSERVACIONES** |
| **`Application` depende solo de `Domain`** | `GestorEstudiantes.Application.csproj` solo referencia a `Domain`. | **SIN OBSERVACIONES** |
| **`Infrastructure` depende de `Application` y `Domain`** | `GestorEstudiantes.Infrastructure.csproj` referencia a `Application` y `Domain`. | **SIN OBSERVACIONES** |
| **`Presentation` usa `Infrastructure` solo en Composition Root** | Solo `App.xaml.cs` importa `GestorEstudiantes.Infrastructure.DependencyInjection`. | **SIN OBSERVACIONES** |
| **Ausencia de ciclos y persistencia externa** | Grafo acíclico verificado y almacenamiento 100% en `List<Estudiante>` en memoria. | **SIN OBSERVACIONES** |
| **Fases 1 a 4 intactas** | Ningún archivo de `Domain`, `Application`, `Infrastructure` o sus 75 pruebas unitarias fue alterado en la Fase 5. | **SIN OBSERVACIONES** |

---

## 9. Problemas Encontrados y Clasificación de Hallazgos

- **Hallazgos `CRÍTICO`:** `0`
- **Hallazgos `IMPORTANTE`:** `0`
- **Hallazgos `MENOR`:** `1`
  1. **[MENOR] Dependencia `_buscarEstudianteUseCase` inyectada pero no invocada en `MainViewModel.cs`:**
     - **Ubicación:** [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs#L22) y líneas 51, 57.
     - **Detalle:** Como el filtrado por matrícula, nombre y apellido en la pantalla se realiza en tiempo real a través de `ICollectionView` (`EstudiantesView.Filter`), el campo `_buscarEstudianteUseCase` queda sin llamadas dentro de `MainViewModel`.

---

## 10. Pruebas Realizadas y Limitaciones (Diferenciación por Nivel)

1. **Inspección estática del código:**
   - **Completada al 100%.** Se verificaron todas las líneas de XAML y C# de `GestorEstudiantes.Presentation`, comprobando enlaces `{Binding}`, `UpdateSourceTrigger=PropertyChanged`, bloqueo de matrícula (`IsEnabled="{Binding MatriculaHabilitada}"`), `DataTrigger` de errores y flujo de comandos.
2. **Compilación y Pruebas Unitarias Automatizadas:**
   - **Completadas al 100%.** `dotnet build` compila los 7 proyectos con `0 Advertencias` y `0 Errores`. `dotnet test` ejecuta y aprueba las **75 pruebas unitarias** (`47 Domain` + `14 Application` + `14 Infrastructure`).
3. **Inicio del proceso WPF en tiempo de ejecución:**
   - **Completado y verificado.** Se lanzó el ejecutable `GestorEstudiantes.Presentation.exe`, confirmando que `App.OnStartup` construye el contenedor IoC, resuelve `MainWindow` con `MainViewModel` y mantiene el proceso activo sin excepciones de carga XAML ni de inyección de dependencias.
4. **Interacción manual real (clics de ratón y escritura de teclado en la ventana gráfica):**
   - **Limitación del entorno de auditoría:** El agente opera desde un entorno de terminal sin controlador de automatización de puntero/teclado interactivo sobre ventanas nativas de escritorio WPF, por lo que las pruebas manuales interactivas sobre la pantalla deben ser realizadas visualmente por el usuario en Visual Studio (`F5`). No se simulan ni declaran como ejecutadas manualmente por el agente.

---

## 11. Recomendaciones de Corrección (Opcionales)

1. **(Opcional - Nivel MENOR) Eliminar el parámetro `BuscarEstudianteUseCase` del constructor de `MainViewModel`:**
   - Si se desea que `MainViewModel` reciba estrictamente los 4 casos de uso que invoca (`RegistrarEstudianteUseCase`, `ObtenerEstudiantesUseCase`, `ActualizarEstudianteUseCase` y `EliminarEstudianteUseCase`), se puede retirar `_buscarEstudianteUseCase` de `MainViewModel.cs` (conservando `BuscarEstudianteUseCase` en la capa `Application` y sus pruebas unitarias intactas).

---

## 12. Veredicto Final

**VEREDICTO: FASE 5 APROBABLE (0 hallazgos críticos, 0 hallazgos importantes, 1 observación menor de limpieza opcional).**

La capa `GestorEstudiantes.Presentation` cumple con el patrón **MVVM** puro, utiliza correctamente `INotifyPropertyChanged`, `RelayCommand`, `ObservableCollection<EstudianteDto>` e `ICollectionView`, mantiene el *code-behind* de `MainWindow.xaml.cs` completamente libre de lógica de negocio y respeta todas las reglas de **Clean Architecture**, **Onion Architecture** y **Hexagonal Architecture**.
