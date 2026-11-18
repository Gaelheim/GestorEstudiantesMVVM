# 15. Corrección Puntual — Eliminación de Dependencia No Utilizada en `MainViewModel`

## 1. Motivo de la Corrección

Durante la Auditoría Final de la Fase 5 ([14_AUDITORIA_FASE_5.md](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/docs/14_AUDITORIA_FASE_5.md)) se identificó una observación de nivel **MENOR**: `MainViewModel` recibía en su constructor el caso de uso `BuscarEstudianteUseCase` y lo almacenaba en el campo privado `_buscarEstudianteUseCase`, pero nunca lo invocaba debido a que el filtrado interactivo por matrícula, nombre y apellido en la tabla se realiza en memoria de la vista mediante `ICollectionView` (`EstudiantesView.Filter = FiltrarEstudiante`).

Con el fin de mantener la máxima limpieza y simplicidad académica en `MainViewModel`, se eliminó exclusivamente esa dependencia no utilizada del ViewModel.

---

## 2. Archivos Modificados

- **Único archivo de código modificado:** [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs)

*(Ningún otro archivo de código, XAML, proyecto `.csproj`, solución `.sln`, prueba unitaria o documentación previa fue modificado).*

---

## 3. Cambios Puntuales Realizados en `MainViewModel.cs`

1. **Eliminación del campo privado:** Se retiró `private readonly BuscarEstudianteUseCase _buscarEstudianteUseCase;`.
2. **Eliminación del parámetro en el constructor:** Se retiró `BuscarEstudianteUseCase buscarEstudianteUseCase` de la firma de `public MainViewModel(...)`.
3. **Eliminación de la asignación en el constructor:** Se retiró `_buscarEstudianteUseCase = buscarEstudianteUseCase ?? throw new ArgumentNullException(nameof(buscarEstudianteUseCase));`.
4. **Constructor resultante ([MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs#L47-L57)):**
   ```csharp
   public MainViewModel(
       RegistrarEstudianteUseCase registrarEstudianteUseCase,
       ObtenerEstudiantesUseCase obtenerEstudiantesUseCase,
       ActualizarEstudianteUseCase actualizarEstudianteUseCase,
       EliminarEstudianteUseCase eliminarEstudianteUseCase)
   {
       _registrarEstudianteUseCase = registrarEstudianteUseCase ?? throw new ArgumentNullException(nameof(registrarEstudianteUseCase));
       _obtenerEstudiantesUseCase = obtenerEstudiantesUseCase ?? throw new ArgumentNullException(nameof(obtenerEstudiantesUseCase));
       _actualizarEstudianteUseCase = actualizarEstudianteUseCase ?? throw new ArgumentNullException(nameof(actualizarEstudianteUseCase));
       _eliminarEstudianteUseCase = eliminarEstudianteUseCase ?? throw new ArgumentNullException(nameof(eliminarEstudianteUseCase));
   ```

---

## 4. Confirmación sobre `BuscarEstudianteUseCase` y el Contenedor DI (`App.xaml.cs`)

- **`BuscarEstudianteUseCase` permanece intacto en `GestorEstudiantes.Application`:** El archivo [BuscarEstudianteUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/BuscarEstudianteUseCase.cs) y sus pruebas unitarias en `GestorEstudiantes.Application.Tests` no fueron tocados.
- **Permanece registrado en el Composition Root ([App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs#L32)):** La línea `services.AddTransient<BuscarEstudianteUseCase>();` se conserva en `ConfigureServices`.
- **Resolución automática de `MainViewModel`:** Dado que `MainViewModel` se registra mediante `services.AddTransient<MainViewModel>();`, `Microsoft.Extensions.DependencyInjection` inspecciona automáticamente el constructor actualizado de 4 parámetros y lo resuelve sin necesidad de modificar `App.xaml.cs`.

---

## 5. Resultado de la Compilación y 6. Resultado de las Pruebas Unitarias

1. **`dotnet restore GestorEstudiantesMVVM.sln`:** Restauración completada sin errores.
2. **`dotnet build GestorEstudiantesMVVM.sln`:** **`Compilación correcta. 0 Advertencia(s), 0 Errores.`**
3. **`dotnet test GestorEstudiantesMVVM.sln`:**
   - `GestorEstudiantes.Domain.Tests`: **47/47 superadas**
   - `GestorEstudiantes.Application.Tests`: **14/14 superadas**
   - `GestorEstudiantes.Infrastructure.Tests`: **14/14 superadas**
   - **Total:** **75/75 pruebas unitarias aprobadas.**

---

## 7. Confirmación de Integridad y 8. Incidencias Encontradas

- **Confirmación:** No se modificó ninguna propiedad, comando, método, evento, validación ni el filtrado con `ICollectionView` en `MainViewModel`, ni se tocó ningún otro proyecto de la solución.
- **Incidencias encontradas:** Ninguna.
