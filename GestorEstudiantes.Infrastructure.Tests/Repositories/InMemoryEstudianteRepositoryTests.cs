using GestorEstudiantes.Domain.Entities;
using GestorEstudiantes.Infrastructure.Repositories;

namespace GestorEstudiantes.Infrastructure.Tests.Repositories;

public class InMemoryEstudianteRepositoryTests
{
    [Fact]
    public void ObtenerTodos_CuandoColeccionEstaVacia_DevuelveListaVacia()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();

        // Act
        var estudiantes = repository.ObtenerTodos();

        // Assert
        Assert.NotNull(estudiantes);
        Assert.Empty(estudiantes);
    }

    [Fact]
    public void ObtenerTodos_CuandoExistenRegistros_DevuelveTodosSinExponerListaInterna()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));
        repository.Agregar(Estudiante.Create("2026-0002", "Ana", "Gómez", "ana@universidad.edu", "Medicina"));

        // Act
        var listaDevuelta = repository.ObtenerTodos();
        listaDevuelta.Clear(); // Modificar la lista devuelta no debe afectar la colección interna del repositorio

        // Assert
        var listaInternaActual = repository.ObtenerTodos();
        Assert.Equal(2, listaInternaActual.Count);
        Assert.Equal("2026-0001", listaInternaActual[0].Matricula);
        Assert.Equal("2026-0002", listaInternaActual[1].Matricula);
    }

    [Fact]
    public void BuscarPorMatricula_ConMatriculaExistente_DevuelveEstudianteCorrespondiente()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        var estudiante = Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");
        repository.Agregar(estudiante);

        // Act
        var encontrado = repository.BuscarPorMatricula("2026-0001");

        // Assert
        Assert.NotNull(encontrado);
        Assert.Equal("2026-0001", encontrado.Matricula);
        Assert.Equal("Juan", encontrado.Nombre);
    }

    [Fact]
    public void BuscarPorMatricula_ConMatriculaInexistente_DevuelveNull()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        // Act
        var encontrado = repository.BuscarPorMatricula("9999-9999");

        // Assert
        Assert.Null(encontrado);
    }

    [Fact]
    public void BuscarPorNombre_ConCoincidenciaParcial_DevuelveEstudiantesCoincidentes()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "María Fernanda", "López", "maria@universidad.edu", "Ingeniería"));
        repository.Agregar(Estudiante.Create("2026-0002", "Fernanda Sofía", "Gómez", "fernanda@universidad.edu", "Derecho"));
        repository.Agregar(Estudiante.Create("2026-0003", "Carlos", "Martínez", "carlos@universidad.edu", "Medicina"));

        // Act
        var resultados = repository.BuscarPorNombre("Fernan");

        // Assert
        Assert.Equal(2, resultados.Count);
        Assert.Contains(resultados, e => e.Matricula == "2026-0001");
        Assert.Contains(resultados, e => e.Matricula == "2026-0002");
    }

    [Fact]
    public void BuscarPorNombre_IgnorandoMayusculasYMinusculas_DevuelveCoincidencias()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Alejandro", "Ramírez", "alejandro@universidad.edu", "Ingeniería"));

        // Act
        var resultadosMinusculas = repository.BuscarPorNombre("alejan");
        var resultadosMayusculas = repository.BuscarPorNombre("ALEJANDRO");

        // Assert
        Assert.Single(resultadosMinusculas);
        Assert.Equal("2026-0001", resultadosMinusculas[0].Matricula);
        Assert.Single(resultadosMayusculas);
        Assert.Equal("2026-0001", resultadosMayusculas[0].Matricula);
    }

    [Fact]
    public void BuscarPorNombre_SinCoincidencias_DevuelveColeccionVacia()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        // Act
        var resultados = repository.BuscarPorNombre("Guillermo");

        // Assert
        Assert.NotNull(resultados);
        Assert.Empty(resultados);
    }

    [Fact]
    public void Agregar_EstudianteValido_LoIncorporaALaColeccionEnMemoria()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        var estudiante = Estudiante.Create("2026-0010", "Laura", "Méndez", "laura@universidad.edu", "Arquitectura");

        // Act
        repository.Agregar(estudiante);

        // Assert
        var todos = repository.ObtenerTodos();
        Assert.Single(todos);
        Assert.Equal("2026-0010", todos[0].Matricula);
        Assert.Equal("Laura", todos[0].Nombre);
    }

    [Fact]
    public void Actualizar_EstudianteExistente_ActualizaSusDatosEnLaColeccionConservandoMatricula()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        var original = Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería");
        repository.Agregar(original);

        var modificado = Estudiante.Create("2026-0001", "Juan Alberto", "Pérez Soto", "juan.alberto@universidad.edu", "Ciencias de la Computación");

        // Act
        repository.Actualizar(modificado);

        // Assert
        var almacenado = repository.BuscarPorMatricula("2026-0001");
        Assert.NotNull(almacenado);
        Assert.Equal("2026-0001", almacenado.Matricula);
        Assert.Equal("Juan Alberto", almacenado.Nombre);
        Assert.Equal("Pérez Soto", almacenado.Apellido);
        Assert.Equal("juan.alberto@universidad.edu", almacenado.CorreoElectronico);
        Assert.Equal("Ciencias de la Computación", almacenado.Carrera);
        Assert.Single(repository.ObtenerTodos());
    }

    [Fact]
    public void Actualizar_EstudianteInexistente_LanzaInvalidOperationExceptionYNoAlteraColeccion()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        var inexistente = Estudiante.Create("9999-9999", "Pedro", "Gómez", "pedro@universidad.edu", "Derecho");

        // Act & Assert
        var excepcion = Assert.Throws<InvalidOperationException>(() => repository.Actualizar(inexistente));
        Assert.Contains("9999-9999", excepcion.Message);
        Assert.Single(repository.ObtenerTodos());
        Assert.False(repository.ExisteMatricula("9999-9999"));
    }

    [Fact]
    public void Eliminar_EstudianteExistente_LoRemueveDeLaColeccion()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));
        repository.Agregar(Estudiante.Create("2026-0002", "Ana", "Gómez", "ana@universidad.edu", "Medicina"));

        // Act
        repository.Eliminar("2026-0001");

        // Assert
        Assert.False(repository.ExisteMatricula("2026-0001"));
        Assert.Null(repository.BuscarPorMatricula("2026-0001"));
        var restantes = repository.ObtenerTodos();
        Assert.Single(restantes);
        Assert.Equal("2026-0002", restantes[0].Matricula);
    }

    [Fact]
    public void Eliminar_EstudianteInexistente_NoProduceErrorNiModificaRegistrosExistentes()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        // Act
        repository.Eliminar("9999-9999");

        // Assert
        var restantes = repository.ObtenerTodos();
        Assert.Single(restantes);
        Assert.Equal("2026-0001", restantes[0].Matricula);
    }

    [Fact]
    public void ExisteMatricula_ConMatriculaRegistrada_DevuelveTrue()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        // Act
        var existe = repository.ExisteMatricula("2026-0001");

        // Assert
        Assert.True(existe);
    }

    [Fact]
    public void ExisteMatricula_ConMatriculaInexistente_DevuelveFalse()
    {
        // Arrange
        var repository = new InMemoryEstudianteRepository();
        repository.Agregar(Estudiante.Create("2026-0001", "Juan", "Pérez", "juan@universidad.edu", "Ingeniería"));

        // Act
        var existe = repository.ExisteMatricula("9999-9999");

        // Assert
        Assert.False(existe);
    }
}
