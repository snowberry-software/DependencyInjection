namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// <see cref="IServiceFactory"/> that is used for scopes.
/// </summary>
/// <remarks>The root scope will always be <see langword="null"/>.</remarks>
public interface IServiceFactoryScoped
{
    /// <summary>
    /// Notifies the <see cref="IServiceFactoryScoped"/> that a new scope has been created.
    /// </summary>
    /// <param name="scope">The scope that has been created.</param>
    void NotifyScopeCreated(IScope scope);

    /// <summary>
    /// Notifies the <see cref="IServiceFactoryScoped"/> that a new scope has been disposed.
    /// </summary>
    /// <param name="scope">The scope that has been disposed.</param>
    void NotifyScopeDisposed(IScope? scope);

    /// <summary>
    /// Requests the instance for the given <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the requested service.</param>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <returns>The requested service instance.</returns>
    object GetService(Type serviceType, IScope scope);

    /// <summary>
    /// Requests the instance for the given <typeparamref name="T"/> service type.
    /// </summary>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The requested service instance as <typeparamref name="T"/>.</returns>
    T GetService<T>(IScope scope);

    /// <summary>
    /// Requests the instance for the given optional <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the requested service.</param>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <returns>The requested service instance.</returns>
    object? GetOptionalService(Type serviceType, IScope scope);

    /// <summary>
    /// Requests the instance for the given optional <typeparamref name="T"/> service type.
    /// </summary>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The requested service instance as <typeparamref name="T"/>.</returns>
    T? GetOptionalService<T>(IScope scope);

    /// <summary>
    /// Creates a new instance of the given <paramref name="type"/> and injects the services during initialization.
    /// </summary>
    /// <param name="type">The type to instantiate.</param>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <returns>The instantiated instance.</returns>
    object CreateInstance(Type type, IScope scope);

    /// <summary>
    /// Creates a new instance of the given <typeparamref name="T"/> and injects the services during initialization.
    /// </summary>
    /// <param name="scope">The scope where the service descriptor lives in.</param>
    /// <typeparam name="T">The type to instantiate.</param>
    /// <returns>The instantiated instance as <typeparamref name="T"/>.</returns>
    T CreateInstance<T>(IScope scope);
}
