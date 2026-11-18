namespace GestorEstudiantes.Domain.Exceptions;


// Error que se produce en la capa Domain cuando alguno de los datos de un estudiante
// (matrícula, nombre, apellido, correo o carrera) está vacío o tiene un formato incorrecto.

public sealed class ValidacionEstudianteException : DomainException
{
    public ValidacionEstudianteException(string message) : base(message)
    {
    }

    public ValidacionEstudianteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
