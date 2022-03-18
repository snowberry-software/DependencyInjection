namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Receives the <see cref="IServiceDescriptor"/> from a given registered service.
/// </summary>
public interface IServiceDescriptorReceiver : IDisposeableContainer
{
    /// <summary>
    /// Gets the service descriptor for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>The service descriptor for the specified type.</returns>
    IServiceDescriptor GetServiceDescriptor(Type serviceType);

    /// <summary>
    /// Gets the optional service descriptor for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>The optional service descriptor for the specified type.</returns>
    IServiceDescriptor? GetOptionalServiceDescriptor(Type serviceType);

    /// <summary>
    /// Gets the service descriptor for the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <returns>The service descriptor for the specified type.</returns>
    IServiceDescriptor GetServiceDescriptor<T>();

    /// <summary>
    /// Gets the optional service descriptor for the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <returns>The optional service descriptor for the specified type.</returns>
    IServiceDescriptor? GetOptionalServiceDescriptor<T>();
}
