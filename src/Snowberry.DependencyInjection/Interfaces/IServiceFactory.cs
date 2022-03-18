namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// The factory that will be used to retrieve service instances.
/// </summary>
public interface IServiceFactory : IServiceProvider
{
    /// <summary>
    /// Requests the instance for the given <typeparamref name="T"/> service type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The requested service instance as <typeparamref name="T"/>.</returns>
    T GetService<T>();

    /// <summary>
    /// Requests the instance for the given optional <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the requested service.</param>
    /// <returns>The requested service instance.</returns>
    object? GetOptionalService(Type serviceType);

    /// <summary>
    /// Requests the instance for the given optional <typeparamref name="T"/> service type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The requested service instance as <typeparamref name="T"/>.</returns>
    T? GetOptionalService<T>();

    /// <summary>
    /// Creates a new instance of the given <paramref name="type"/> and injects the services during initialization.
    /// </summary>
    /// <param name="type">The type to instantiate.</param>
    /// <returns>The instantiated instance.</returns>
    object CreateInstance(Type type);

    /// <summary>
    /// Creates a new instance of the given <typeparamref name="T"/> and injects the services during initialization.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</param>
    /// <returns>The instantiated instance as <typeparamref name="T"/>.</returns>
    T CreateInstance<T>();
}
