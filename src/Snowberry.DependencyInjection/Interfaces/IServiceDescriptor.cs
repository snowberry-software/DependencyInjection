namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Represents the data of a registered service.
/// </summary>
public interface IServiceDescriptor
{
    /// <summary>
    /// Clones the <see cref="IServiceDescriptor"/> for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <remarks>The <see cref="SingletonInstance"/> will be ignored.</remarks>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The cloned <see cref="IServiceDescriptor"/>.</returns>
    IServiceDescriptor CloneFor(Type serviceType);

    /// <summary>
    /// The instance of the <see cref="ImplementationType"/>.
    /// </summary>
    object? SingletonInstance { get; set; }

    /// <summary>
    /// The registered service type.
    /// </summary>
    Type ServiceType { get; }

    /// <summary>
    /// The type that represents the implementation of the service.
    /// </summary>
    Type ImplementationType { get; }

    /// <summary>
    /// The lifetime of the service.
    /// </summary>
    ServiceLifetime Lifetime { get; }
}
