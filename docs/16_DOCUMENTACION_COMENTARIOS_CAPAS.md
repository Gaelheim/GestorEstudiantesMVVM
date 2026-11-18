# 16. Documentación del Código — Responsabilidad de Cada Capa

## 1. Objetivo de la Tarea

Se incorporaron comentarios explicativos en español (tanto comentarios de documentación XML `///` como comentarios de línea `//` y comentarios XAML `<!-- -->`) en los archivos principales de las cuatro capas de la solución **GestorEstudiantesMVVM**, con un lenguaje sencillo, claro y didáctico para que un estudiante pueda comprender y explicar fácilmente qué hace cada capa y cada clase sin alterar en absoluto la lógica ni el comportamiento del sistema.

---

## 2. Archivos que Recibieron Comentarios

### A. Capa `GestorEstudiantes.Domain`
1. [Estudiante.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Domain/Entities/Estudiante.cs)
2. [DomainException.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Domain/Exceptions/DomainException.cs)
3. [ValidacionEstudianteException.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Domain/Exceptions/ValidacionEstudianteException.cs)

### B. Capa `GestorEstudiantes.Application`
4. [IEstudianteRepository.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/Interfaces/IEstudianteRepository.cs)
5. [EstudianteDto.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/DTOs/EstudianteDto.cs)
6. [RegistrarEstudianteUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/RegistrarEstudianteUseCase.cs)
7. [ObtenerEstudiantesUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/ObtenerEstudiantesUseCase.cs)
8. [BuscarEstudianteUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/BuscarEstudianteUseCase.cs)
9. [ActualizarEstudianteUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/ActualizarEstudianteUseCase.cs)
10. [EliminarEstudianteUseCase.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Application/UseCases/EliminarEstudianteUseCase.cs)

### C. Capa `GestorEstudiantes.Infrastructure`
11. [InMemoryEstudianteRepository.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/Repositories/InMemoryEstudianteRepository.cs)
12. [InfrastructureServiceRegistration.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Infrastructure/DependencyInjection/InfrastructureServiceRegistration.cs)

### D. Capa `GestorEstudiantes.Presentation`
13. [MainViewModel.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/ViewModels/MainViewModel.cs)
14. [RelayCommand.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Commands/RelayCommand.cs)
15. [MainWindow.xaml](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml)
16. [MainWindow.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/Views/MainWindow.xaml.cs)
17. [App.xaml.cs](file:///c:/Users/Jari_/Music/GestorEstudiantesMVVM/GestorEstudiantes.Presentation/App.xaml.cs)

---

## 3. Responsabilidad Explicada en Cada Capa

1. **`GestorEstudiantes.Domain`:**
   - Se explicó que contiene la información principal del sistema (`Estudiante`) y sus reglas de validación (comprobar que los datos sean obligatorios, limpiar espacios y verificar el formato del correo electrónico), sin depender de las demás capas ni de cómo se muestre o almacene el estudiante.
2. **`GestorEstudiantes.Application`:**
   - Se explicó que coordina las operaciones que puede realizar el sistema (registrar, consultar, buscar, actualizar y eliminar estudiantes) apoyándose en las reglas de `Domain`.
   - Se documentó el papel de `EstudianteDto` como objeto sencillo de transporte de datos hacia la interfaz gráfica y de `IEstudianteRepository` como contrato que define las operaciones necesarias para acceder a los estudiantes sin decidir cómo se guardan.
3. **`GestorEstudiantes.Infrastructure`:**
   - Se explicó que proporciona la implementación concreta del almacenamiento mediante `InMemoryEstudianteRepository`, cumpliendo el contrato `IEstudianteRepository` definido en `Application`.
   - Se aclaró que utiliza una lista (`List<Estudiante>`) en memoria RAM para guardar los estudiantes temporalmente mientras el programa está abierto, y que los datos se pierden al cerrar la aplicación.
4. **`GestorEstudiantes.Presentation`:**
   - Se explicó que es la capa con la que interactúa directamente el usuario, desarrollada con WPF y estructurada con el patrón **MVVM**:
     - **View (`MainWindow.xaml` y `MainWindow.xaml.cs`):** Muestra la tabla, el formulario, la barra de búsqueda y los mensajes, enviando las acciones del usuario mediante *Bindings* y comandos.
     - **ViewModel (`MainViewModel.cs`):** Controla los datos mostrados en pantalla, filtra la colección observable y coordina las llamadas a los casos de uso de `Application`.
     - **Command (`RelayCommand.cs`):** Permite ejecutar acciones desde los botones de la ventana y controlar cuándo están habilitados.
     - **Inicio y Configuración (`App.xaml.cs`):** Registra los servicios y conecta las dependencias necesarias para iniciar la aplicación.

---

## 4. Tipo de Comentarios Añadidos

- **Comentarios XML (`/// <summary>...</summary>`):** En clases, interfaces, eventos, propiedades relevantes y métodos públicos para que Visual Studio también muestre las descripciones en las ayudas contextuales (*IntelliSense*).
- **Comentarios de bloque y de línea (`// ...` y `<!-- ... -->`):** Al inicio de los archivos principales de cada capa y en secciones clave del código para explicar de forma breve y directa qué hace cada bloque.

---

## 5. Confirmación de Integridad de la Lógica y Resultados de Verificación

- **Confirmación:** No se modificó ninguna instrucción ejecutable, lógica de negocio, validación, propiedad, constructor, comando, *Binding*, inyección de dependencias, referencia entre proyectos ni prueba unitaria.
- **Resultado de Compilación (`dotnet build GestorEstudiantesMVVM.sln`):**
  - **`Compilación correcta. 0 Advertencia(s), 0 Errores.`**
- **Resultado de Pruebas Unitarias (`dotnet test GestorEstudiantesMVVM.sln`):**
  - `GestorEstudiantes.Domain.Tests`: **47/47 superadas**
  - `GestorEstudiantes.Application.Tests`: **14/14 superadas**
  - `GestorEstudiantes.Infrastructure.Tests`: **14/14 superadas**
  - **Total:** **75/75 pruebas unitarias aprobadas.**
