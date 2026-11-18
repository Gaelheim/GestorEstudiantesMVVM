using GestorEstudiantes.Application.Interfaces;

namespace GestorEstudiantes.Application.UseCases;


// Caso de uso de la capa Application que se encarga de eliminar a un estudiante del sistema
// después de comprobar que su matrícula existe.

public class EliminarEstudianteUseCase
{
    private readonly IEstudianteRepository _repository;

    public EliminarEstudianteUseCase(IEstudianteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // Elimina al estudiante que corresponda a la matrícula indicada.
   
    public void Ejecutar(string matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            throw new InvalidOperationException("Debe indicar la matrícula del estudiante que desea eliminar.");
        }

        var matriculaNormalizada = matricula.Trim();

        // Se verifica primero que el estudiante exista antes de pedir su eliminación.
        if (!_repository.ExisteMatricula(matriculaNormalizada))
        {
            throw new InvalidOperationException($"No se encontró ningún estudiante con la matrícula '{matriculaNormalizada}'.");
        }

        _repository.Eliminar(matriculaNormalizada);
    }
}
