namespace GestorEstudiantes.Domain.Exceptions;


// Clase base para los errores relacionados con las reglas principales de la capa Domain.
// Permite identificar cuando una operación falla por incumplir una regla del sistema.

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
