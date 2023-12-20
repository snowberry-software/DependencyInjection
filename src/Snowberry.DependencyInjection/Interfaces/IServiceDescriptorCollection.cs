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
    /// <param name="serviceKey">The optional service key.</param>
    /// <returns>The service descriptor for the specified type.</returns>
    IServiceDescriptor GetServiceDescriptor(Type serviceType, object? serviceKey);

    /// <summary>
    /// Gets the optional service descriptor for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <param name="serviceKey">The optional service key.</param>
    /// <returns>The optional service descriptor for the specified type.</returns>
    IServiceDescriptor? GetOptionalServiceDescriptor(Type serviceType, object? serviceKey);

    /// <summary>
    /// Gets the service descriptor for the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="serviceKey">The optional service key.</param>
    /// <returns>The service descriptor for the specified type.</returns>
    IServiceDescriptor GetServiceDescriptor<T>(object? serviceKey);

    /// <summary>
    /// Gets the optional service descriptor for the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="serviceKey">The optional service key.</param>
    /// <returns>The optional service descriptor for the specified type.</returns>
    IServiceDescriptor? GetOptionalServiceDescriptor<T>(object? serviceKey);
}
