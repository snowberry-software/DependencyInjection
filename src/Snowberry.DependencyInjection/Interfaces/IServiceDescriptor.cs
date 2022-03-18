namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Represents the data of a registered service.
/// </summary>
public interface IServiceDescriptor
{
    /// <summary>
    /// The instance of the <see cref="ImplementationType"/>.
    /// </summary>
    public object? SingletonInstance { get; set; }

    /// <summary>
    /// The registered service type.
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// The type that represents the implementation of the service.
    /// </summary>
    public Type ImplementationType { get; }

    /// <summary>
    /// The lifetime of the service.
    /// </summary>
    public ServiceLifetime Lifetime { get; }
}
