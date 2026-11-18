using System.Windows;
using GestorEstudiantes.Presentation.ViewModels;

namespace GestorEstudiantes.Presentation.Views;

/// <summary>
/// Ventana principal de la interfaz gráfica (View en el patrón MVVM).
/// Muestra la información en pantalla y delega todas las acciones al <see cref="MainViewModel"/>
/// conectado en su DataContext.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        // Conecta la vista con el ViewModel para que funcionen los enlaces de datos (Bindings) y comandos.
        DataContext = viewModel;
    }
}
