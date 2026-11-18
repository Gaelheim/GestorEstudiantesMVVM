using GestorEstudiantes.Application.DTOs;
using GestorEstudiantes.Application.Interfaces;

namespace GestorEstudiantes.Application.UseCases;


// Caso de uso de la capa Application que coordina la actualización de los datos de un estudiante.
// Busca al estudiante por su matrícula, aplica las reglas de validación de Domain y guarda los cambios.

public class ActualizarEstudianteUseCase
{
    private readonly IEstudianteRepository _repository;

    public ActualizarEstudianteUseCase(IEstudianteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

  
    // Actualiza el nombre, apellido, correo y carrera de un estudiante existente.
  
    public EstudianteDto Ejecutar(
        string matricula,
        string nombre,
        string apellido,
        string correoElectronico,
        string carrera)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            throw new InvalidOperationException("Debe indicar la matrícula del estudiante que desea actualizar.");
        }

        // 1. Se busca al estudiante en el repositorio utilizando su matrícula.
        var matriculaNormalizada = matricula.Trim();
        var estudiante = _repository.BuscarPorMatricula(matriculaNormalizada);

        if (estudiante == null)
        {
            throw new InvalidOperationException($"No se encontró ningún estudiante con la matrícula '{matriculaNormalizada}'.");
        }

        // 2. La entidad de Domain valida y actualiza sus propiedades manteniendo la matrícula sin cambios.
        estudiante.ActualizarDatos(nombre, apellido, correoElectronico, carrera);

        // 3. Se guardan los cambios utilizando el repositorio.
        _repository.Actualizar(estudiante);

        return new EstudianteDto
        {
            Matricula = estudiante.Matricula,
            Nombre = estudiante.Nombre,
            Apellido = estudiante.Apellido,
            CorreoElectronico = estudiante.CorreoElectronico,
            Carrera = estudiante.Carrera
        };
    }
}
