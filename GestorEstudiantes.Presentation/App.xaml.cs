using System.Windows;
using GestorEstudiantes.Application.UseCases;
using GestorEstudiantes.Infrastructure.DependencyInjection;
using GestorEstudiantes.Presentation.ViewModels;
using GestorEstudiantes.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GestorEstudiantes.Presentation;

/// <summary>
/// Punto de inicio de la aplicación WPF.
/// Se encarga de configurar los servicios, conectar las dependencias de todas las capas
/// y abrir la ventana principal.
/// </summary>
public partial class App : System.Windows.Application
{
    public IServiceProvider ServiceProvider { get; }

    public App()
    {
        // Se crea y construye el contenedor con todas las dependencias registradas.
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    // Registra el repositorio, los casos de uso, el ViewModel y la ventana principal.
    private static void ConfigureServices(IServiceCollection services)
    {
        // 1. Registro del repositorio en memoria (Singleton) desde la capa Infrastructure.
        services.AddInfrastructureServices();

        // 2. Registro de los cinco casos de uso de la capa Application.
        services.AddTransient<RegistrarEstudianteUseCase>();
        services.AddTransient<ObtenerEstudiantesUseCase>();
        services.AddTransient<BuscarEstudianteUseCase>();
        services.AddTransient<ActualizarEstudianteUseCase>();
        services.AddTransient<EliminarEstudianteUseCase>();

        // 3. Registro del ViewModel y de la ventana principal de la capa Presentation (MVVM).
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Se obtiene la ventana principal con sus dependencias ya conectadas y se muestra en pantalla.
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
