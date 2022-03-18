namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Implements both service factories (<see cref="IServiceFactory"/>, <see cref="IServiceFactoryScoped"/>).
/// </summary>
public interface IScopedServiceFactory : IServiceFactory, IServiceFactoryScoped
{
}
