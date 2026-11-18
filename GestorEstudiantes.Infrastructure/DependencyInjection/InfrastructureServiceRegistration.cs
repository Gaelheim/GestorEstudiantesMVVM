using GestorEstudiantes.Application.Interfaces;
using GestorEstudiantes.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GestorEstudiantes.Infrastructure.DependencyInjection;


// Registra los servicios de la capa Infrastructure en el contenedor de dependencias.
// Indica que cuando la aplicación necesite <see cref="IEstudianteRepository"/>, se utilice <see cref="InMemoryEstudianteRepository"/>.

public static class InfrastructureServiceRegistration
{
    
    // Registra el repositorio en memoria como una única instancia compartida (Singleton) durante toda la ejecución.
   
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEstudianteRepository, InMemoryEstudianteRepository>();
        return services;
    }
}
