namespace GestorEstudiantes.Application.DTOs;


public class EstudianteDto
{
    // Datos básicos del estudiante que se envían hacia la interfaz de usuario sin exponer directamente la clase Estudiante de la capa Domain
    public string Matricula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
}
