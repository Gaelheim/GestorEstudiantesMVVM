using GestorEstudiantes.Application.Tests.Fakes;
using GestorEstudiantes.Application.UseCases;
using GestorEstudiantes.Domain.Exceptions;

namespace GestorEstudiantes.Application.Tests.UseCases;

public class EstudianteUseCasesTests
{
  
    // PRUEBAS DE REGISTRO 
  

    [Fact]
    public void RegistrarEstudiante_ConDatosValidos_RegistraYRetornaDto()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var useCase = new RegistrarEstudianteUseCase(repository);

        // Act
        var resultado = useCase.Ejecutar(
            " 2026-0001 ",
            " Juan ",
            " Pérez ",
            " juan.perez@universidad.edu ",
            " Ingeniería de Software ");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("2026-0001", resultado.Matricula);
        Assert.Equal("Juan", resultado.Nombre);
        Assert.Equal("Pérez", resultado.Apellido);
        Assert.Equal("juan.perez@universidad.edu", resultado.CorreoElectronico);
        Assert.Equal("Ingeniería de Software", resultado.Carrera);
        Assert.True(repository.ExisteMatricula("2026-0001"));
    }

    [Fact]
    public void RegistrarEstudiante_ConMatriculaDuplicada_LanzaInvalidOperationException()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var useCase = new RegistrarEstudianteUseCase(repository);

        useCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act & Assert
        var excepcion = Assert.Throws<InvalidOperationException>(() =>
            useCase.Ejecutar("  2026-0001  ", "María", "Gómez", "maria@universidad.edu", "Arquitectura"));

        Assert.Contains("2026-0001", excepcion.Message);
        Assert.Single(repository.ObtenerTodos());
    }

    [Fact]
    public void RegistrarEstudiante_ConDatosInvalidos_PropagaValidacionEstudianteExceptionDeDomain()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var useCase = new RegistrarEstudianteUseCase(repository);

        // Act & Assert
        Assert.Throws<ValidacionEstudianteException>(() =>
            useCase.Ejecutar("2026-0001", "", "Pérez", "correo-invalido", "Ingeniería"));

        Assert.Empty(repository.ObtenerTodos());
    }

   
    // PRUEBAS DE CONSULTA Y BÚSQUEDA 
    

    [Fact]
    public void ObtenerEstudiantes_ConEstudiantesRegistrados_RetornaListaDeDtos()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var obtenerUseCase = new ObtenerEstudiantesUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");
        registrarUseCase.Ejecutar("2026-0002", "Ana", "Gómez", "ana@universidad.edu", "Medicina");

        // Act
        var lista = obtenerUseCase.Ejecutar();

        // Assert
        Assert.Equal(2, lista.Count);
        Assert.Equal("2026-0001", lista[0].Matricula);
        Assert.Equal("2026-0002", lista[1].Matricula);
    }

    [Fact]
    public void ObtenerEstudiantes_SinEstudiantesRegistrados_RetornaListaVacia()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var obtenerUseCase = new ObtenerEstudiantesUseCase(repository);

        // Act
        var lista = obtenerUseCase.Ejecutar();

        // Assert
        Assert.NotNull(lista);
        Assert.Empty(lista);
    }

    [Fact]
    public void BuscarPorMatricula_ConMatriculaExistente_RetornaEstudianteDto()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var buscarUseCase = new BuscarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act
        var encontrado = buscarUseCase.BuscarPorMatricula(" 2026-0001 ");

        // Assert
        Assert.NotNull(encontrado);
        Assert.Equal("2026-0001", encontrado.Matricula);
        Assert.Equal("Juan", encontrado.Nombre);
    }

    [Fact]
    public void BuscarPorMatricula_ConMatriculaInexistente_RetornaNull()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var buscarUseCase = new BuscarEstudianteUseCase(repository);

        // Act
        var encontrado = buscarUseCase.BuscarPorMatricula("9999-9999");

        // Assert
        Assert.Null(encontrado);
    }

    [Fact]
    public void BuscarPorNombre_ConCoincidencias_RetornaListaDeCoincidencias()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var buscarUseCase = new BuscarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan Carlos", "Pérez", "juan@universidad.edu", "Ingeniería");
        registrarUseCase.Ejecutar("2026-0002", "Juana", "Martínez", "juana@universidad.edu", "Derecho");
        registrarUseCase.Ejecutar("2026-0003", "Pedro", "Gómez", "pedro@universidad.edu", "Medicina");

        // Act
        var coincidencias = buscarUseCase.BuscarPorNombre("Juan");

        // Assert
        Assert.Equal(2, coincidencias.Count);
    }

    [Fact]
    public void BuscarPorNombre_SinCoincidencias_RetornaListaVacia()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var buscarUseCase = new BuscarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act
        var coincidencias = buscarUseCase.BuscarPorNombre("Roberto");

        // Assert
        Assert.NotNull(coincidencias);
        Assert.Empty(coincidencias);
    }

    
    // PRUEBAS DE ACTUALIZACIÓN 


    [Fact]
    public void ActualizarEstudiante_ExistenteConDatosValidos_ActualizaDatosYConservaMatriculaOriginal()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var actualizarUseCase = new ActualizarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act
        var actualizado = actualizarUseCase.Ejecutar(
            "2026-0001",
            "Juan Alberto",
            "Pérez López",
            "juan.alberto@universidad.edu",
            "Ciencias de Datos");

        // Assert
        Assert.Equal("2026-0001", actualizado.Matricula);
        Assert.Equal("Juan Alberto", actualizado.Nombre);
        Assert.Equal("Pérez López", actualizado.Apellido);
        Assert.Equal("juan.alberto@universidad.edu", actualizado.CorreoElectronico);
        Assert.Equal("Ciencias de Datos", actualizado.Carrera);

        var enRepositorio = repository.BuscarPorMatricula("2026-0001");
        Assert.NotNull(enRepositorio);
        Assert.Equal("2026-0001", enRepositorio.Matricula);
        Assert.Equal("Juan Alberto", enRepositorio.Nombre);
    }

    [Fact]
    public void ActualizarEstudiante_Inexistente_LanzaInvalidOperationException()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var actualizarUseCase = new ActualizarEstudianteUseCase(repository);

        // Act & Assert
        var excepcion = Assert.Throws<InvalidOperationException>(() =>
            actualizarUseCase.Ejecutar("9999-9999", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        Assert.Contains("9999-9999", excepcion.Message);
    }

    [Fact]
    public void ActualizarEstudiante_ConDatosInvalidos_PropagaValidacionEstudianteExceptionYConservaDatosPrevios()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var actualizarUseCase = new ActualizarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act & Assert
        Assert.Throws<ValidacionEstudianteException>(() =>
            actualizarUseCase.Ejecutar("2026-0001", "Juan Nuevo", "Pérez Nuevo", "correo-invalido", "Nueva Carrera"));

        var enRepositorio = repository.BuscarPorMatricula("2026-0001")!;
        Assert.Equal("Juan", enRepositorio.Nombre);
        Assert.Equal("Pérez", enRepositorio.Apellido);
        Assert.Equal("juan@universidad.edu", enRepositorio.CorreoElectronico);
        Assert.Equal("Ingeniería", enRepositorio.Carrera);
    }

   
    //PRUEBAS DE ELIMINACIÓN 
   

    [Fact]
    public void EliminarEstudiante_Existente_LoEliminaDelRepositorio()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var registrarUseCase = new RegistrarEstudianteUseCase(repository);
        var eliminarUseCase = new EliminarEstudianteUseCase(repository);

        registrarUseCase.Ejecutar("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");

        // Act
        eliminarUseCase.Ejecutar("2026-0001");

        // Assert
        Assert.False(repository.ExisteMatricula("2026-0001"));
        Assert.Empty(repository.ObtenerTodos());
    }

    [Fact]
    public void EliminarEstudiante_Inexistente_LanzaInvalidOperationException()
    {
        // Arrange
        var repository = new FakeEstudianteRepository();
        var eliminarUseCase = new EliminarEstudianteUseCase(repository);

        // Act & Assert
        var excepcion = Assert.Throws<InvalidOperationException>(() =>
            eliminarUseCase.Ejecutar("9999-9999"));

        Assert.Contains("9999-9999", excepcion.Message);
    }
}
