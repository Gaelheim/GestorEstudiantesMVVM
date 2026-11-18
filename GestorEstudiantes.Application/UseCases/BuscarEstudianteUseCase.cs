using GestorEstudiantes.Application.DTOs;
using GestorEstudiantes.Application.Interfaces;

namespace GestorEstudiantes.Application.UseCases;


// Caso de uso de la capa Application que permite buscar estudiantes por matrícula o por nombre,
//devolviendo los resultados como objetos EstudianteDto.

public class BuscarEstudianteUseCase
{
    private readonly IEstudianteRepository _repository;

    public BuscarEstudianteUseCase(IEstudianteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

  
    // Busca un estudiante por su matrícula. Devuelve null si no se encuentra.
   
    public EstudianteDto? BuscarPorMatricula(string matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            return null;
        }

        // Se consulta al repositorio con la matrícula sin espacios sobrantes.
        var estudiante = _repository.BuscarPorMatricula(matricula.Trim());
        if (estudiante == null)
        {
            return null;
        }

        return new EstudianteDto
        {
            Matricula = estudiante.Matricula,
            Nombre = estudiante.Nombre,
            Apellido = estudiante.Apellido,
            CorreoElectronico = estudiante.CorreoElectronico,
            Carrera = estudiante.Carrera
        };
    }

   
    // Busca todos los estudiantes cuyo nombre coincida con el texto recibido.
  
    public List<EstudianteDto> BuscarPorNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return new List<EstudianteDto>();
        }

        // Se solicitan las coincidencias al repositorio y se transforman a DTOs.
        var estudiantes = _repository.BuscarPorNombre(nombre.Trim());
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
