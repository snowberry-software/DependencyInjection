namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Represents a registry to register services.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Registers the given service.
    /// </summary>
    /// <param name="serviceType">The service to register.</param>
    /// <param name="implementationType">The implementation type of the service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    /// <param name="singletonInstance">The optional singleton instance.</param>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry Register(Type serviceType, Type implementationType, ServiceLifetime lifetime, object? singletonInstance);

    /// <summary>
    /// Creates and registers a singleton service of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterSingleton<T>();

    /// <summary>
    /// Registers a singleton service of the type <typeparamref name="T"/> using the given <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">The instance that will be returned when requesting the <typeparamref name="T"/> service.</param>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterSingleton<T>(T instance);

    /// <summary>
    /// Creates and registers a singleton service of the type <typeparamref name="TImpl"/> for the service <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <typeparam name="TImpl">The implementation of the service.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterSingleton<T, TImpl>() where TImpl : T;

    /// <summary>
    /// Registers a singleton service of the type <typeparamref name="TImpl"/> for the service <typeparamref name="T"/> using the given <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">The instance that will be returned when requesting the <typeparamref name="T"/> service.</param>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <typeparam name="TImpl">The implementation of the service.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterSingleton<T, TImpl>(TImpl instance) where TImpl : T;

    /// <summary>
    /// Creates and registers a transient service of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterTransient<T>();

    /// <summary>
    /// Creates and registers a transient service of the type <typeparamref name="TImpl"/> for the service <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <typeparam name="TImpl">The implementation of the service.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterTransient<T, TImpl>() where TImpl : T;

    /// <summary>
    /// Creates and registers a scoped service of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterScoped<T>();

    /// <summary>
    /// Creates and registers a scoped service of the type <typeparamref name="TImpl"/> for the service <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The service to register.</typeparam>
    /// <typeparam name="TImpl">The implementation of the service.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry RegisterScoped<T, TImpl>() where TImpl : T;

    /// <summary>
    /// Creates a new scope which can be used to resolve scoped services.
    /// </summary>
    /// <returns>The instance of the scope.</returns>
    IScope CreateScope();

    /// <summary>
    /// Unregisters a registered service of the type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>All disposeable instances from <typeparamref name="T"/> will be disposed as usual, except if it is a service with the lifetime of a <see cref="ServiceLifetime.Singleton"/>.
    /// <para/>
    /// A singleton service will be disposed directly within the unregister method.
    /// </remarks>
    /// <param name="successful">Determines whether the service was successfully unregistered.</param>
    /// <typeparam name="T">The service to unregister.</typeparam>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry UnregisterService<T>(out bool successful);

    /// <summary>
    /// Unregisters a registered service of the type <paramref name="serviceType"/>.
    /// </summary>
    /// <remarks>
    /// All disposeable instances from <paramref name="serviceType"/> will be disposed as usual, except if it is a service with the lifetime of a <see cref="ServiceLifetime.Singleton"/>.
    /// <para/>
    /// A singleton service will be disposed directly within the unregister method.
    /// </remarks>
    /// <param name="serviceType">The service to unregister.</param>
    /// <param name="successful">Determines whether the service was successfully unregistered.</param>
    /// <returns>The current <see cref="IServiceRegistry"/> for chaining calls.</returns>
    IServiceRegistry UnregisterService(Type serviceType, out bool successful);

    /// <summary>
    /// Checks whether the given <paramref name="serviceType"/> is registered.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>Whether the given <paramref name="serviceType"/> is registered or not.</returns>
    bool IsServiceRegistered(Type serviceType);

    /// <summary>
    /// Checks whether the given <typeparamref name="T"/> is registered.
    /// </summary>
    /// <typeparam name="T">The type of the service.</param>
    /// <returns>Whether the given <typeparamref name="T"/> is registered or not.</returns>
    bool IsServiceRegistered<T>();

    /// <summary>
    /// Gets the amount of registered services.
    /// </summary>
    int Count { get; }
}
