using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using GestorEstudiantes.Application.DTOs;
using GestorEstudiantes.Application.UseCases;
using GestorEstudiantes.Domain.Exceptions;
using GestorEstudiantes.Presentation.Commands;

namespace GestorEstudiantes.Presentation.ViewModels;


// CAPA PRESENTATION (GestorEstudiantes.Presentation)
// Esta es la capa con la que interactúa directamente el usuario.
// Utiliza WPF y el patrón MVVM para separar la ventana visual (View) de la
// clase que controla los datos en pantalla y llama a los casos de uso (ViewModel).



// ViewModel principal de la ventana (patrón MVVM).
// Controla los datos que se muestran en la pantalla, recibe las acciones de los botones
// y coordina las operaciones llamando a los casos de uso de la capa Application.

public class MainViewModel : INotifyPropertyChanged
{
    // Casos de uso de la capa Application que realizan las operaciones del sistema.
    private readonly RegistrarEstudianteUseCase _registrarEstudianteUseCase;
    private readonly ObtenerEstudiantesUseCase _obtenerEstudiantesUseCase;
    private readonly ActualizarEstudianteUseCase _actualizarEstudianteUseCase;
    private readonly EliminarEstudianteUseCase _eliminarEstudianteUseCase;

    // Comandos que conectan los botones de la ventana con los métodos de este ViewModel.
    private readonly RelayCommand _guardarCommand;
    private readonly RelayCommand _limpiarCommand;
    private readonly RelayCommand _eliminarCommand;
    private readonly RelayCommand _limpiarBusquedaCommand;

    // Campos privados que guardan el estado actual del formulario, la búsqueda y los mensajes.
    private string _matricula = string.Empty;
    private string _nombre = string.Empty;
    private string _apellido = string.Empty;
    private string _correoElectronico = string.Empty;
    private string _carrera = string.Empty;
    private string _textoBusqueda = string.Empty;
    private string _mensaje = "Listo. Complete el formulario para registrar un estudiante o seleccione uno de la tabla.";
    private bool _esError;
    private bool _esEdicion;
    private EstudianteDto? _estudianteSeleccionado;

 
    // Evento que avisa a la ventana WPF cada vez que cambia el valor de una propiedad.
 
    public event PropertyChangedEventHandler? PropertyChanged;

    
    // Lista observable de estudiantes que actualiza automáticamente la tabla en pantalla cuando cambia.
  
    public ObservableCollection<EstudianteDto> Estudiantes { get; } = new();

    
    // Vista de la colección de estudiantes que permite filtrar la tabla sin borrar los datos originales.
   
    public ICollectionView EstudiantesView { get; }

    public MainViewModel(
        RegistrarEstudianteUseCase registrarEstudianteUseCase,
        ObtenerEstudiantesUseCase obtenerEstudiantesUseCase,
        ActualizarEstudianteUseCase actualizarEstudianteUseCase,
        EliminarEstudianteUseCase eliminarEstudianteUseCase)
    {
        _registrarEstudianteUseCase = registrarEstudianteUseCase ?? throw new ArgumentNullException(nameof(registrarEstudianteUseCase));
        _obtenerEstudiantesUseCase = obtenerEstudiantesUseCase ?? throw new ArgumentNullException(nameof(obtenerEstudiantesUseCase));
        _actualizarEstudianteUseCase = actualizarEstudianteUseCase ?? throw new ArgumentNullException(nameof(actualizarEstudianteUseCase));
        _eliminarEstudianteUseCase = eliminarEstudianteUseCase ?? throw new ArgumentNullException(nameof(eliminarEstudianteUseCase));

        // Se configura la vista de la tabla para que utilice el método FiltrarEstudiante al buscar.
        EstudiantesView = CollectionViewSource.GetDefaultView(Estudiantes);
        EstudiantesView.Filter = FiltrarEstudiante;

        // Se inicializan los comandos que ejecutarán los botones de la interfaz.
        _guardarCommand = new RelayCommand(_ => Guardar());
        _limpiarCommand = new RelayCommand(_ => Limpiar());
        _eliminarCommand = new RelayCommand(_ => Eliminar(), _ => EsEdicion && EstudianteSeleccionado != null);
        _limpiarBusquedaCommand = new RelayCommand(_ => TextoBusqueda = string.Empty);

        CargarEstudiantes();
    }

    // Propiedades enlazadas (Data Binding) a las cajas de texto del formulario.
    public string Matricula
    {
        get => _matricula;
        set
        {
            if (_matricula != value)
            {
                _matricula = value;
                OnPropertyChanged();
            }
        }
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre != value)
            {
                _nombre = value;
                OnPropertyChanged();
            }
        }
    }

    public string Apellido
    {
        get => _apellido;
        set
        {
            if (_apellido != value)
            {
                _apellido = value;
                OnPropertyChanged();
            }
        }
    }

    public string CorreoElectronico
    {
        get => _correoElectronico;
        set
        {
            if (_correoElectronico != value)
            {
                _correoElectronico = value;
                OnPropertyChanged();
            }
        }
    }

    public string Carrera
    {
        get => _carrera;
        set
        {
            if (_carrera != value)
            {
                _carrera = value;
                OnPropertyChanged();
            }
        }
    }

 
    // Texto escrito en el cuadro de búsqueda. Al cambiar, refresca el filtro de la tabla inmediatamente.
   
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (_textoBusqueda != value)
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                EstudiantesView.Refresh();
            }
        }
    }

   
    // Mensaje informativo o de error que se muestra en la parte inferior de la ventana.
   
    public string Mensaje
    {
        get => _mensaje;
        private set
        {
            if (_mensaje != value)
            {
                _mensaje = value;
                OnPropertyChanged();
            }
        }
    }

   
    // Indica si el mensaje actual corresponde a un error para cambiar el color del panel en la vista.
  
    public bool EsError
    {
        get => _esError;
        private set
        {
            if (_esError != value)
            {
                _esError = value;
                OnPropertyChanged();
            }
        }
    }

   
    // Indica si el formulario está editando un estudiante existente (true) o registrando uno nuevo (false).
  
    public bool EsEdicion
    {
        get => _esEdicion;
        private set
        {
            if (_esEdicion != value)
            {
                _esEdicion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MatriculaHabilitada));
                OnPropertyChanged(nameof(TituloFormulario));
                _eliminarCommand.RaiseCanExecuteChanged();
            }
        }
    }

   
    // Permite escribir la matrícula solo al registrar un estudiante nuevo y la bloquea durante la edición.
   
    public bool MatriculaHabilitada => !EsEdicion;

     // Título dinámico que indica en pantalla si se está registrando o editando un estudiante.
    
    public string TituloFormulario => EsEdicion
        ? "Edición de Estudiante (Matrícula inmutable)"
        : "Registro de Nuevo Estudiante";

 
    // Estudiante seleccionado en la tabla. Al seleccionarlo, sus datos se copian al formulario para editarlos.
    
    public EstudianteDto? EstudianteSeleccionado
    {
        get => _estudianteSeleccionado;
        set
        {
            if (_estudianteSeleccionado != value)
            {
                _estudianteSeleccionado = value;
                OnPropertyChanged();

                if (_estudianteSeleccionado != null)
                {
                    Matricula = _estudianteSeleccionado.Matricula;
                    Nombre = _estudianteSeleccionado.Nombre;
                    Apellido = _estudianteSeleccionado.Apellido;
                    CorreoElectronico = _estudianteSeleccionado.CorreoElectronico;
                    Carrera = _estudianteSeleccionado.Carrera;
                    EsEdicion = true;
                    MostrarMensaje($"Estudiante '{_estudianteSeleccionado.Matricula}' cargado en modo edición.", esError: false);
                }

                _eliminarCommand.RaiseCanExecuteChanged();
            }
        }
    }

    // Comandos públicos a los que se enlazan los botones en MainWindow.xaml.
    public ICommand GuardarCommand => _guardarCommand;
    public ICommand LimpiarCommand => _limpiarCommand;
    public ICommand EliminarCommand => _eliminarCommand;
    public ICommand LimpiarBusquedaCommand => _limpiarBusquedaCommand;

    // Consulta la lista actualizada de estudiantes desde Application y refresca la colección de la vista.
    private void CargarEstudiantes()
    {
        Estudiantes.Clear();
        var lista = _obtenerEstudiantesUseCase.Ejecutar();
        foreach (var estudiante in lista)
        {
            Estudiantes.Add(estudiante);
        }
        EstudiantesView.Refresh();
    }

    // Registra un estudiante nuevo o actualiza el estudiante seleccionado según el estado de EsEdicion.
    private void Guardar()
    {
        try
        {
            if (!EsEdicion)
            {
                var registrado = _registrarEstudianteUseCase.Ejecutar(
                    Matricula,
                    Nombre,
                    Apellido,
                    CorreoElectronico,
                    Carrera);

                CargarEstudiantes();
                LimpiarFormularioInterno();
                MostrarMensaje($"Estudiante con matrícula '{registrado.Matricula}' registrado correctamente.", esError: false);
            }
            else
            {
                if (EstudianteSeleccionado == null)
                {
                    MostrarMensaje("Seleccione un estudiante de la tabla para poder actualizar sus datos.", esError: true);
                    return;
                }

                var matriculaOriginal = EstudianteSeleccionado.Matricula;

                var actualizado = _actualizarEstudianteUseCase.Ejecutar(
                    matriculaOriginal,
                    Nombre,
                    Apellido,
                    CorreoElectronico,
                    Carrera);

                CargarEstudiantes();
                LimpiarFormularioInterno();
                MostrarMensaje($"Datos del estudiante '{actualizado.Matricula}' actualizados correctamente.", esError: false);
            }
        }
        catch (DomainException ex)
        {
            // Muestra los errores de validación de la capa Domain (campos vacíos o correo inválido).
            MostrarMensaje(ex.Message, esError: true);
        }
        catch (InvalidOperationException ex)
        {
            // Muestra los errores de operación de la capa Application (como matrícula duplicada).
            MostrarMensaje(ex.Message, esError: true);
        }
        catch (Exception)
        {
            MostrarMensaje("Ocurrió un error inesperado al procesar la operación.", esError: true);
        }
    }

    // Elimina al estudiante que se encuentra actualmente seleccionado en la tabla.
    private void Eliminar()
    {
        if (!EsEdicion || EstudianteSeleccionado == null)
        {
            MostrarMensaje("Debe seleccionar un estudiante de la tabla para eliminarlo.", esError: true);
            return;
        }

        try
        {
            var matriculaAEliminar = EstudianteSeleccionado.Matricula;
            _eliminarEstudianteUseCase.Ejecutar(matriculaAEliminar);

            CargarEstudiantes();
            LimpiarFormularioInterno();
            MostrarMensaje($"Estudiante con matrícula '{matriculaAEliminar}' eliminado correctamente.", esError: false);
        }
        catch (DomainException ex)
        {
            MostrarMensaje(ex.Message, esError: true);
        }
        catch (InvalidOperationException ex)
        {
            MostrarMensaje(ex.Message, esError: true);
        }
        catch (Exception)
        {
            MostrarMensaje("Ocurrió un error inesperado al intentar eliminar el estudiante.", esError: true);
        }
    }

    // Limpia el formulario y muestra un mensaje indicando que está listo para un nuevo registro.
    private void Limpiar()
    {
        LimpiarFormularioInterno();
        MostrarMensaje("Formulario limpio. Listo para registrar un nuevo estudiante.", esError: false);
    }

    // Vacía las cajas de texto, quita la selección de la tabla y vuelve al modo de registro.
    private void LimpiarFormularioInterno()
    {
        _estudianteSeleccionado = null;
        OnPropertyChanged(nameof(EstudianteSeleccionado));

        Matricula = string.Empty;
        Nombre = string.Empty;
        Apellido = string.Empty;
        CorreoElectronico = string.Empty;
        Carrera = string.Empty;

        EsEdicion = false;
        _eliminarCommand.RaiseCanExecuteChanged();
    }

    // Comprueba si un estudiante coincide con el texto escrito en la barra de búsqueda (por matrícula, nombre o apellido).
    private bool FiltrarEstudiante(object item)
    {
        if (item is not EstudianteDto estudiante)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            return true;
        }

        var criterio = TextoBusqueda.Trim();

        return estudiante.Matricula.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
               estudiante.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
               estudiante.Apellido.Contains(criterio, StringComparison.OrdinalIgnoreCase);
    }

    // Actualiza el mensaje y el indicador de error que se muestran en la barra inferior.
    private void MostrarMensaje(string texto, bool esError)
    {
        Mensaje = texto;
        EsError = esError;
    }

    // Notifica a la interfaz WPF que una propiedad ha cambiado para que actualice el control en pantalla.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
