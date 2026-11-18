using GestorEstudiantes.Application.DTOs;
using GestorEstudiantes.Application.Interfaces;
using GestorEstudiantes.Domain.Entities;

namespace GestorEstudiantes.Application.UseCases;


// Caso de uso de la capa Application que coordina el registro de un nuevo estudiante.
// Utiliza la clase Estudiante de Domain para validar los datos y el repositorio para comprobar
// que la matrícula no esté repetida antes de guardarlo.

public class RegistrarEstudianteUseCase
{
    private readonly IEstudianteRepository _repository;

    public RegistrarEstudianteUseCase(IEstudianteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }


    // Registra un nuevo estudiante y devuelve sus datos en un EstudianteDto.

    public EstudianteDto Ejecutar(
        string matricula,
        string nombre,
        string apellido,
        string correoElectronico,
        string carrera)
    {
        // 1. La entidad de Domain valida y normaliza todos los datos (incluida la matrícula).
        var estudiante = Estudiante.Create(
            matricula,
            nombre,
            apellido,
            correoElectronico,
            carrera);

        // 2. Application comprueba que no exista otro estudiante con la misma matrícula.
        if (_repository.ExisteMatricula(estudiante.Matricula))
        {
            throw new InvalidOperationException($"Ya existe un estudiante registrado con la matrícula '{estudiante.Matricula}'.");
        }

        // 3. Se guarda el estudiante mediante el contrato del repositorio.
        _repository.Agregar(estudiante);

        // 4. Se devuelven los datos registrados dentro de un DTO para la interfaz gráfica.
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
