using GestorEstudiantes.Application.DTOs;
using GestorEstudiantes.Application.Interfaces;

namespace GestorEstudiantes.Application.UseCases;


// Caso de uso de la capa Application que solicita todos los estudiantes registrados
// y los convierte a objetos EstudianteDto para que puedan mostrarse en la interfaz.

public class ObtenerEstudiantesUseCase
{
    private readonly IEstudianteRepository _repository;

    public ObtenerEstudiantesUseCase(IEstudianteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }


    // Devuelve la lista de todos los estudiantes registrados convertidos a EstudianteDto.
  
    public List<EstudianteDto> Ejecutar()
    {
        // Se piden los estudiantes al repositorio y se copian sus datos a una lista de DTOs.
        var estudiantes = _repository.ObtenerTodos();
        var resultado = new List<EstudianteDto>();

        foreach (var estudiante in estudiantes)
        {
            resultado.Add(new EstudianteDto
            {
                Matricula = estudiante.Matricula,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                CorreoElectronico = estudiante.CorreoElectronico,
                Carrera = estudiante.Carrera
            });
        }

        return resultado;
    }
}
