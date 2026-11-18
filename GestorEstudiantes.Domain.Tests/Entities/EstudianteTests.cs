using System.Reflection;
using GestorEstudiantes.Domain.Entities;
using GestorEstudiantes.Domain.Exceptions;

namespace GestorEstudiantes.Domain.Tests.Entities;

public class EstudianteTests
{
    [Fact]
    public void Create_ConDatosValidos_CreaEstudianteCorrectamente()
    {
        // Arrange
        var matricula = "2026-0001";
        var nombre = "Juan";
        var apellido = "Pérez";
        var correo = "juan.perez@universidad.edu";
        var carrera = "Ingeniería de Software";

        // Act
        var estudiante = Estudiante.Create(matricula, nombre, apellido, correo, carrera);

        // Assert
        Assert.NotNull(estudiante);
        Assert.Equal("2026-0001", estudiante.Matricula);
        Assert.Equal("Juan", estudiante.Nombre);
        Assert.Equal("Pérez", estudiante.Apellido);
        Assert.Equal("juan.perez@universidad.edu", estudiante.CorreoElectronico);
        Assert.Equal("Ingeniería de Software", estudiante.Carrera);
    }

    [Fact]
    public void Create_ConEspaciosAlInicioYFinal_NormalizaTodosLosCampos()
    {
        // Arrange
        var matricula = "   2026-0042   ";
        var nombre = "  María Fernanda  ";
        var apellido = "   Gómez López ";
        var correo = "  maria.gomez@campus.universidad.edu.do   ";
        var carrera = "   Ciencias de la Computación  ";

        // Act
        var estudiante = Estudiante.Create(matricula, nombre, apellido, correo, carrera);

        // Assert
        Assert.Equal("2026-0042", estudiante.Matricula);
        Assert.Equal("María Fernanda", estudiante.Nombre);
        Assert.Equal("Gómez López", estudiante.Apellido);
        Assert.Equal("maria.gomez@campus.universidad.edu.do", estudiante.CorreoElectronico);
        Assert.Equal("Ciencias de la Computación", estudiante.Carrera);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\r\n")]
    public void Create_ConMatriculaVaciaONula_LanzaValidacionEstudianteException(string? matriculaInvalida)
    {
        // Act & Assert
        var excepcion = Assert.Throws<ValidacionEstudianteException>(() =>
            Estudiante.Create(matriculaInvalida!, "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        Assert.Contains("matrícula", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ConNombreVacioONulo_LanzaValidacionEstudianteException(string? nombreInvalido)
    {
        // Act & Assert
        var excepcion = Assert.Throws<ValidacionEstudianteException>(() =>
            Estudiante.Create("2026-0001", nombreInvalido!, "Pérez", "juan@universidad.edu", "Ingeniería"));

        Assert.Contains("nombre", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ConApellidoVacioONulo_LanzaValidacionEstudianteException(string? apellidoInvalido)
    {
        // Act & Assert
        var excepcion = Assert.Throws<ValidacionEstudianteException>(() =>
            Estudiante.Create("2026-0001", "Juan", apellidoInvalido!, "juan@universidad.edu", "Ingeniería"));

        Assert.Contains("apellido", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("correosinArroba.com")]
    [InlineData("@dominio.com")]
    [InlineData("usuario@")]
    [InlineData("usuario@dominio")]
    [InlineData("usuario@.dominio.com")]
    [InlineData("usuario@dominio.com.")]
    [InlineData("usuario@dominio..com")]
    [InlineData("usuario..nombre@dominio.com")]
    [InlineData(".usuario@dominio.com")]
    [InlineData("usuario.@dominio.com")]
    [InlineData("usuario@@dominio.com")]
    [InlineData("usuario con espacio@dominio.com")]
    [InlineData("usuario@dominio con espacio.com")]
    public void Create_ConCorreoInvalido_LanzaValidacionEstudianteException(string? correoInvalido)
    {
        // Act & Assert
        var excepcion = Assert.Throws<ValidacionEstudianteException>(() =>
            Estudiante.Create("2026-0001", "Juan", "Pérez", correoInvalido!, "Ingeniería"));

        Assert.Contains("correo electrónico", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("estudiante@universidad.edu")]
    [InlineData("nombre.apellido+tag@sub.dominio.edu.mx")]
    [InlineData("user_123-abc@facultad.org")]
    public void Create_ConFormatosDeCorreoLegitimos_PermiteCreacion(string correoLegitimo)
    {
        // Act
        var estudiante = Estudiante.Create("2026-0001", "Juan", "Pérez", correoLegitimo, "Ingeniería");

        // Assert
        Assert.Equal(correoLegitimo, estudiante.CorreoElectronico);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ConCarreraVaciaONula_LanzaValidacionEstudianteException(string? carreraInvalida)
    {
        // Act & Assert
        var excepcion = Assert.Throws<ValidacionEstudianteException>(() =>
            Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", carreraInvalida!));

        Assert.Contains("carrera", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ActualizarNombre_ConNombreValido_ActualizaYNormalizaCorrectamente()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act
        estudiante.ActualizarNombre("  Carlos Alberto  ");

        // Assert
        Assert.Equal("Carlos Alberto", estudiante.Nombre);
    }

    [Fact]
    public void ActualizarApellido_ConApellidoValido_ActualizaYNormalizaCorrectamente()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act
        estudiante.ActualizarApellido("  Ramírez Santos  ");

        // Assert
        Assert.Equal("Ramírez Santos", estudiante.Apellido);
    }

    [Fact]
    public void ActualizarCorreoElectronico_ConCorreoValido_ActualizaYNormalizaCorrectamente()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act
        estudiante.ActualizarCorreoElectronico("  carlos.ramirez@nuevo.edu  ");

        // Assert
        Assert.Equal("carlos.ramirez@nuevo.edu", estudiante.CorreoElectronico);
    }

    [Fact]
    public void ActualizarCarrera_ConCarreraValida_ActualizaYNormalizaCorrectamente()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act
        estudiante.ActualizarCarrera("  Ingeniería en Ciberseguridad  ");

        // Assert
        Assert.Equal("Ingeniería en Ciberseguridad", estudiante.Carrera);
    }

    [Fact]
    public void ActualizarDatos_ConDatosValidos_ActualizaPropiedadesModificablesYConservaMatricula()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();
        var matriculaOriginal = estudiante.Matricula;

        // Act
        estudiante.ActualizarDatos(
            "  Ana Lucía ",
            " Martínez ",
            " ana.martinez@universidad.edu ",
            " Arquitectura de Software ");

        // Assert
        Assert.Equal(matriculaOriginal, estudiante.Matricula);
        Assert.Equal("Ana Lucía", estudiante.Nombre);
        Assert.Equal("Martínez", estudiante.Apellido);
        Assert.Equal("ana.martinez@universidad.edu", estudiante.CorreoElectronico);
        Assert.Equal("Arquitectura de Software", estudiante.Carrera);
    }

    [Fact]
    public void Modificaciones_ConValoresInvalidos_LanzanExcepcionYNoAlteranEstadoPrevio()
    {
        // Arrange
        var estudiante = Estudiante.Create(
            "2026-0001",
            "Juan",
            "Pérez",
            "juan.perez@universidad.edu",
            "Ingeniería de Software");

        // Act & Assert
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarNombre("   "));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarApellido(""));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarCorreoElectronico("correo-invalido"));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarCarrera("   "));
        Assert.Throws<ValidacionEstudianteException>(() =>
            estudiante.ActualizarDatos("Nuevo Nombre", "Nuevo Apellido", "correo_malo", "Nueva Carrera"));

        // El estado original permanece íntegro tras intentos fallidos de modificación
        Assert.Equal("2026-0001", estudiante.Matricula);
        Assert.Equal("Juan", estudiante.Nombre);
        Assert.Equal("Pérez", estudiante.Apellido);
        Assert.Equal("juan.perez@universidad.edu", estudiante.CorreoElectronico);
        Assert.Equal("Ingeniería de Software", estudiante.Carrera);
    }

    [Theory]
    [InlineData("", "Nuevo Apellido", "nuevo.correo@universidad.edu", "Nueva Carrera")]
    [InlineData("Nuevo Nombre", "   ", "nuevo.correo@universidad.edu", "Nueva Carrera")]
    [InlineData("Nuevo Nombre", "Nuevo Apellido", "correo..invalido@dominio.com", "Nueva Carrera")]
    [InlineData("Nuevo Nombre", "Nuevo Apellido", "nuevo.correo@universidad.edu", "")]
    public void ActualizarDatos_CuandoFallaCualquierParametro_EsAtomicoYConservaEstadoOriginal(
        string nuevoNombre,
        string nuevoApellido,
        string nuevoCorreo,
        string nuevaCarrera)
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act & Assert
        Assert.Throws<ValidacionEstudianteException>(() =>
            estudiante.ActualizarDatos(nuevoNombre, nuevoApellido, nuevoCorreo, nuevaCarrera));

        // Ninguna propiedad debe haber cambiado
        Assert.Equal("2026-0001", estudiante.Matricula);
        Assert.Equal("Juan", estudiante.Nombre);
        Assert.Equal("Pérez", estudiante.Apellido);
        Assert.Equal("juan.perez@universidad.edu", estudiante.CorreoElectronico);
        Assert.Equal("Ingeniería de Software", estudiante.Carrera);
    }

    [Fact]
    public void ModificacionesIndividuales_ConValoresNulos_LanzanExcepcionYConservanEstadoOriginal()
    {
        // Arrange
        var estudiante = CrearEstudiantePorDefecto();

        // Act & Assert
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarNombre(null!));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarApellido(null!));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarCorreoElectronico(null!));
        Assert.Throws<ValidacionEstudianteException>(() => estudiante.ActualizarCarrera(null!));

        // El estado original permanece intacto
        Assert.Equal("2026-0001", estudiante.Matricula);
        Assert.Equal("Juan", estudiante.Nombre);
        Assert.Equal("Pérez", estudiante.Apellido);
        Assert.Equal("juan.perez@universidad.edu", estudiante.CorreoElectronico);
        Assert.Equal("Ingeniería de Software", estudiante.Carrera);
    }

    [Fact]
    public void ValidacionEstudianteException_ConInnerException_ConservaMensajeYExcepcionOriginal()
    {
        // Arrange
        var excepcionInterna = new FormatException("Detalle técnico del formato.");
        var mensaje = "Error de validación del estudiante.";

        // Act
        var excepcionDominio = new ValidacionEstudianteException(mensaje, excepcionInterna);

        // Assert
        Assert.Equal(mensaje, excepcionDominio.Message);
        Assert.Same(excepcionInterna, excepcionDominio.InnerException);
        Assert.IsAssignableFrom<DomainException>(excepcionDominio);
    }

    [Fact]
    public void Estudiante_EncapsulamientoEInmutabilidadDeMatricula_NoPermiteModificacionExterna()
    {
        // Arrange
        var tipoEstudiante = typeof(Estudiante);
        var propiedadMatricula = tipoEstudiante.GetProperty(nameof(Estudiante.Matricula))!;
        var propiedadNombre = tipoEstudiante.GetProperty(nameof(Estudiante.Nombre))!;
        var propiedadApellido = tipoEstudiante.GetProperty(nameof(Estudiante.Apellido))!;
        var propiedadCorreo = tipoEstudiante.GetProperty(nameof(Estudiante.CorreoElectronico))!;
        var propiedadCarrera = tipoEstudiante.GetProperty(nameof(Estudiante.Carrera))!;

        // Assert: Matrícula no tiene setter de ningún tipo (inmutable después del constructor)
        Assert.Null(propiedadMatricula.SetMethod);

        // Assert: Las demás propiedades no tienen setter público
        Assert.False(propiedadNombre.SetMethod?.IsPublic ?? false);
        Assert.False(propiedadApellido.SetMethod?.IsPublic ?? false);
        Assert.False(propiedadCorreo.SetMethod?.IsPublic ?? false);
        Assert.False(propiedadCarrera.SetMethod?.IsPublic ?? false);

        // Assert: No existen constructores públicos que permitan evadir Estudiante.Create
        var constructoresPublicos = tipoEstudiante.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Empty(constructoresPublicos);
    }

    private static Estudiante CrearEstudiantePorDefecto()
    {
        return Estudiante.Create(
            "2026-0001",
            "Juan",
            "Pérez",
            "juan.perez@universidad.edu",
            "Ingeniería de Software");
    }
}
