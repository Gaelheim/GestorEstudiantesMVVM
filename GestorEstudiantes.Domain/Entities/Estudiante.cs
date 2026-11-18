using GestorEstudiantes.Domain.Exceptions;

namespace GestorEstudiantes.Domain.Entities;


// CAPA DOMAIN 
// Esta capa contiene la información principal del sistema y las reglas que
// debe cumplir un estudiante, sin depender de las demás capas ni de cómo
// se muestre en pantalla o se guarde en memoria.



// Representa a un estudiante registrado en el sistema.
// Contiene sus datos principales y se encarga de comprobar que siempre sean válidos.

public sealed class Estudiante
{
    // Datos que identifican y describen al estudiante.
    // Solo pueden modificarse mediante los métodos de esta clase para evitar datos incorrectos.
    public string Matricula { get; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string CorreoElectronico { get; private set; }
    public string Carrera { get; private set; }

    // Constructor privado para obligar a que todo estudiante se cree pasando por las validaciones de Create().
    private Estudiante(
        string matricula,
        string nombre,
        string apellido,
        string correoElectronico,
        string carrera)
    {
        Matricula = matricula;
        Nombre = nombre;
        Apellido = apellido;
        CorreoElectronico = correoElectronico;
        Carrera = carrera;
    }

   
    // Crea un nuevo estudiante después de comprobar y limpiar todos sus datos.
 
    public static Estudiante Create(
        string matricula,
        string nombre,
        string apellido,
        string correoElectronico,
        string carrera)
    {
        // Se comprueba que ningún dato obligatorio esté vacío y se quitan espacios sobrantes.
        var matriculaNormalizada = ValidarYNormalizarCampoObligatorio(matricula, "La matrícula");
        var nombreNormalizado = ValidarYNormalizarCampoObligatorio(nombre, "El nombre");
        var apellidoNormalizado = ValidarYNormalizarCampoObligatorio(apellido, "El apellido");
        var correoNormalizado = ValidarYNormalizarCorreo(correoElectronico);
        var carreraNormalizada = ValidarYNormalizarCampoObligatorio(carrera, "La carrera");

        return new Estudiante(
            matriculaNormalizada,
            nombreNormalizado,
            apellidoNormalizado,
            correoNormalizado,
            carreraNormalizada);
    }


    // Actualiza los datos modificables del estudiante conservando su matrícula original.
    // Si algún dato no es válido, lanza una excepción y no cambia nada.
   
    public void ActualizarDatos(
        string nombre,
        string apellido,
        string correoElectronico,
        string carrera)
    {
        // Primero se validan todos los datos nuevos antes de modificar las propiedades del estudiante.
        var nombreNormalizado = ValidarYNormalizarCampoObligatorio(nombre, "El nombre");
        var apellidoNormalizado = ValidarYNormalizarCampoObligatorio(apellido, "El apellido");
        var correoNormalizado = ValidarYNormalizarCorreo(correoElectronico);
        var carreraNormalizada = ValidarYNormalizarCampoObligatorio(carrera, "La carrera");

        Nombre = nombreNormalizado;
        Apellido = apellidoNormalizado;
        CorreoElectronico = correoNormalizado;
        Carrera = carreraNormalizada;
    }


    // Actualiza únicamente el nombre del estudiante tras comprobar que sea válido.
   
    public void ActualizarNombre(string nombre)
    {
        Nombre = ValidarYNormalizarCampoObligatorio(nombre, "El nombre");
    }

 
    // Actualiza únicamente el apellido del estudiante tras comprobar que sea válido.
  
    public void ActualizarApellido(string apellido)
    {
        Apellido = ValidarYNormalizarCampoObligatorio(apellido, "El apellido");
    }


    // Actualiza únicamente el correo electrónico del estudiante tras comprobar su formato.
 
    public void ActualizarCorreoElectronico(string correoElectronico)
    {
        CorreoElectronico = ValidarYNormalizarCorreo(correoElectronico);
    }

  
    // Actualiza únicamente la carrera del estudiante tras comprobar que no esté vacía.
   
    public void ActualizarCarrera(string carrera)
    {
        Carrera = ValidarYNormalizarCampoObligatorio(carrera, "La carrera");
    }

    // Comprueba que un campo de texto no esté vacío y elimina los espacios al inicio y al final.
    private static string ValidarYNormalizarCampoObligatorio(string? valor, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ValidacionEstudianteException($"{nombreCampo} del estudiante es obligatorio y no puede estar vacío.");
        }

        return valor.Trim();
    }

    // Comprueba que el correo no esté vacío y cumpla con un formato válido.
    private static string ValidarYNormalizarCorreo(string? correoElectronico)
    {
        var correoNormalizado = ValidarYNormalizarCampoObligatorio(correoElectronico, "El correo electrónico");

        if (!EsFormatoCorreoValido(correoNormalizado))
        {
            throw new ValidacionEstudianteException("El correo electrónico proporcionado no tiene un formato válido.");
        }

        return correoNormalizado;
    }

    // Reglas sencillas para verificar que el correo tenga forma de dirección electrónica válida (usuario@dominio.ext).
    private static bool EsFormatoCorreoValido(string correo)
    {
        // No debe contener espacios intermedios ni caracteres de control
        if (correo.Any(c => char.IsWhiteSpace(c) || char.IsControl(c)))
        {
            return false;
        }

        var indiceArroba = correo.IndexOf('@');

        // Debe existir exactamente un símbolo '@' y no estar ni al inicio ni al final
        if (indiceArroba <= 0 || indiceArroba != correo.LastIndexOf('@') || indiceArroba == correo.Length - 1)
        {
            return false;
        }

        var parteLocal = correo[..indiceArroba];
        var dominio = correo[(indiceArroba + 1)..];

        if (parteLocal.Length == 0 || dominio.Length < 3)
        {
            return false;
        }

        // La parte local no debe tener puntos al inicio, al final ni puntos consecutivos
        if (parteLocal.StartsWith('.') || parteLocal.EndsWith('.') || parteLocal.Contains(".."))
        {
            return false;
        }

        // El dominio debe contener al menos un punto separador, sin puntos al inicio, al final ni consecutivos
        if (!dominio.Contains('.') || dominio.StartsWith('.') || dominio.EndsWith('.') || dominio.Contains(".."))
        {
            return false;
        }

        return true;
    }
}
