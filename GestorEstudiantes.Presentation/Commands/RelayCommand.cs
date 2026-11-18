using System.Windows.Input;

namespace GestorEstudiantes.Presentation.Commands;


// Permite conectar los botones de la interfaz gráfica (View) con las acciones del ViewModel en MVVM.
// Implementa la interfaz <see cref="ICommand"/> de WPF.

public class RelayCommand : ICommand
{
    // Acción que se ejecutará cuando el usuario pulse el botón.
    private readonly Action<object?> _execute;

    // Función opcional que indica si el botón debe estar habilitado o deshabilitado.
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

   
    // Evento que avisa a WPF cuando cambia el estado de habilitación del comando.
    
    public event EventHandler? CanExecuteChanged;

 
    // Comprueba si el comando puede ejecutarse en el estado actual.
   
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

 
    // Ejecuta la acción asignada al comando.
    
    public void Execute(object? parameter) => _execute(parameter);

    
    // Notifica a la interfaz gráfica que debe volver a comprobar si el botón está habilitado.
   
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
