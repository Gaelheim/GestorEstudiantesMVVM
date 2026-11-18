using GestorEstudiantes.Domain.Entities;

namespace GestorEstudiantes.Application.Interfaces;


// Esta capa define las operaciones que puede realizar el sistema (registrar,
// consultar, buscar, actualizar y eliminar estudiantes) y utiliza las reglas
// de la capa Domain, sin decidir cómo se guardan realmente los datos.



// Define las operaciones que la capa Application necesita para guardar y consultar estudiantes.
// No decide dónde ni cómo se almacenan los datos; esa implementación se realiza en la capa Infrastructure.

public interface IEstudianteRepository
{
   
    // Obtiene la lista completa de estudiantes registrados.

    List<Estudiante> ObtenerTodos();

    // Busca a un estudiante específico por su matrícula. Devuelve null si no existe.
    
    Estudiante? BuscarPorMatricula(string matricula);

    
    //Busca a los estudiantes cuyo nombre o apellido coincida con el texto indicado.
    
    List<Estudiante> BuscarPorNombre(string nombre);


    // Guarda un nuevo estudiante.
   
    void Agregar(Estudiante estudiante);


    // Guarda los cambios realizados en un estudiante existente.
 
    void Actualizar(Estudiante estudiante);

  
    // Elimina a un estudiante a partir de su matrícula.

    void Eliminar(string matricula);


    // Comprueba si ya existe un estudiante registrado con esa matrícula.
 
    bool ExisteMatricula(string matricula);
}
