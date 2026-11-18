using GestorEstudiantes.Application.Interfaces;
using GestorEstudiantes.Domain.Entities;

namespace GestorEstudiantes.Infrastructure.Repositories;


// CAPA INFRASTRUCTURE 
// Esta capa proporciona la implementación concreta de cómo se guardan y
// recuperan los estudiantes. Cumple el contrato IEstudianteRepository definido
// en Application utilizando una lista en memoria que se borra al cerrar el programa.



// Se encarga de guardar, buscar, actualizar y eliminar estudiantes.
// En este proyecto utiliza una lista en memoria (List&lt;Estudiante&gt;), por lo que los datos
// se conservan mientras la aplicación está abierta y se pierden al cerrarla.

public class InMemoryEstudianteRepository : IEstudianteRepository
{
    // Lista privada donde se guardan temporalmente los estudiantes en memoria RAM.
    private readonly List<Estudiante> _estudiantes = new();

   
    // Devuelve una copia de la lista con todos los estudiantes guardados en memoria.
    
    public List<Estudiante> ObtenerTodos()
    {
        return new List<Estudiante>(_estudiantes);
    }

 
    // Busca a un estudiante en la lista comparando su matrícula sin distinguir mayúsculas o minúsculas.
   
    public Estudiante? BuscarPorMatricula(string matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            return null;
        }

        var matriculaBuscada = matricula.Trim();

        return _estudiantes.FirstOrDefault(e =>
            string.Equals(e.Matricula, matriculaBuscada, StringComparison.OrdinalIgnoreCase));
    }

  
    // Devuelve los estudiantes cuyo nombre o apellido contenga el texto buscado.
    
    public List<Estudiante> BuscarPorNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return new List<Estudiante>();
        }

        var criterio = nombre.Trim();

        // Se filtran coincidencias parciales ignorando diferencias entre mayúsculas y minúsculas.
        return _estudiantes
            .Where(e =>
                e.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
                e.Apellido.Contains(criterio, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }


    // Añade un nuevo estudiante a la lista en memoria.

    public void Agregar(Estudiante estudiante)
    {
        ArgumentNullException.ThrowIfNull(estudiante);
        _estudiantes.Add(estudiante);
    }

 
    // Busca la posición del estudiante por su matrícula y reemplaza sus datos en la lista.
 
    public void Actualizar(Estudiante estudiante)
    {
        ArgumentNullException.ThrowIfNull(estudiante);

        var indice = _estudiantes.FindIndex(e =>
            string.Equals(e.Matricula, estudiante.Matricula, StringComparison.OrdinalIgnoreCase));

        if (indice < 0)
        {
            throw new InvalidOperationException($"No se encontró ningún estudiante con la matrícula '{estudiante.Matricula}' para actualizar.");
        }

        _estudiantes[indice] = estudiante;
    }


    // Quita de la lista en memoria al estudiante que tenga la matrícula indicada.
  
    public void Eliminar(string matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            return;
        }

        var estudiante = BuscarPorMatricula(matricula);
        if (estudiante != null)
        {
            _estudiantes.Remove(estudiante);
        }
    }

    
    // Indica si ya hay algún estudiante en la lista con la matrícula recibida.
  
    public bool ExisteMatricula(string matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula))
        {
            return false;
        }

        var matriculaBuscada = matricula.Trim();

        return _estudiantes.Any(e =>
            string.Equals(e.Matricula, matriculaBuscada, StringComparison.OrdinalIgnoreCase));
    }
}
