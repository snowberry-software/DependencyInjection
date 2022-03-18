using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Exceptions;

/// <summary>
/// Gets thrown when the <see cref="IServiceFactory"/> could not instantiate a new instance for the service type.
/// </summary>
public sealed class InvalidServiceImplementationType : Exception
{
    public InvalidServiceImplementationType(Type serviceImplementationType!!) : base($"Cannot instantiate abstract classes or interfaces! ({serviceImplementationType.FullName})")
    {
        ServiceImplementationType = serviceImplementationType;
    }

    public InvalidServiceImplementationType(Type serviceImplementationType!!, string message!!) : base(message)
    {
        ServiceImplementationType = serviceImplementationType;
    }

    /// <summary>
    /// The implementation type of the service.
    /// </summary>
    public Type ServiceImplementationType { get; }
}
