namespace Snowberry.DependencyInjection.Exceptions;

/// <summary>
/// Gets thrown when a requested service is not registered.
/// </summary>
public sealed class ServiceTypeNotRegistered : Exception
{
    public ServiceTypeNotRegistered(Type serviceType!!) : base($"Service type '{serviceType.FullName}' is not registered!")
    {
        ServiceType = serviceType;
    }

    public ServiceTypeNotRegistered(Type serviceType!!, string message) : base(message)
    {
        ServiceType = serviceType;
    }

    /// <summary>
    /// The type of the requested service.
    /// </summary>
    public Type ServiceType { get; }
}
