using GestorEstudiantes.Application.Interfaces;
using GestorEstudiantes.Domain.Entities;

namespace GestorEstudiantes.Application.Tests.Fakes;


// Repositorio falso sencillo en memoria utilizado exclusivamente para probar los casos de uso de Application
// sin depender de la capa Infrastructure ni de librerías externas de mocking.

public class FakeEstudianteRepository : IEstudianteRepository
{
    private readonly List<Estudiante> _estudiantes = new();

    public List<Estudiante> ObtenerTodos()
    {
        return new List<Estudiante>(_estudiantes);
    }

    public Estudiante? BuscarPorMatricula(string matricula)
    {
        return _estudiantes.FirstOrDefault(e =>
            string.Equals(e.Matricula, matricula, StringComparison.OrdinalIgnoreCase));
    }

    public List<Estudiante> BuscarPorNombre(string nombre)
    {
        return _estudiantes
            .Where(e =>
                e.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                e.Apellido.Contains(nombre, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void Agregar(Estudiante estudiante)
    {
        _estudiantes.Add(estudiante);
    }

    public void Actualizar(Estudiante estudiante)
    {
        var indice = _estudiantes.FindIndex(e =>
            string.Equals(e.Matricula, estudiante.Matricula, StringComparison.OrdinalIgnoreCase));

        if (indice >= 0)
        {
            _estudiantes[indice] = estudiante;
        }
    }

    public void Eliminar(string matricula)
    {
        var estudiante = BuscarPorMatricula(matricula);
        if (estudiante != null)
        {
            _estudiantes.Remove(estudiante);
        }
    }

    public bool ExisteMatricula(string matricula)
    {
        return _estudiantes.Any(e =>
            string.Equals(e.Matricula, matricula, StringComparison.OrdinalIgnoreCase));
    }
}
