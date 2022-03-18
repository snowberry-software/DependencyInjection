using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection;

/// <inheritdoc cref="IServiceDescriptor"/>
public class ServiceDescriptor : IServiceDescriptor
{
    protected ServiceDescriptor(Type serviceType!!, Type implementationType!!, ServiceLifetime lifetime, object? singletonInstance)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        SingletonInstance = singletonInstance;
    }

    public static ServiceDescriptor Singleton(Type serviceType!!, Type implementationType!!, object? singletonInstance)
    {
        return new(serviceType, implementationType, ServiceLifetime.Singleton, singletonInstance);
    }

    public static ServiceDescriptor Transient(Type serviceType!!, Type implementationType!!)
    {
        return new(serviceType, implementationType, ServiceLifetime.Transient, null);
    }

    public static ServiceDescriptor Scoped(Type serviceType!!, Type implementationType!!)
    {
        return new(serviceType, implementationType, ServiceLifetime.Scoped, null);
    }

    /// <inheritdoc/>
    public object? SingletonInstance { get; set; }

    /// <inheritdoc/>
    public Type ServiceType { get; }

    /// <inheritdoc/>
    public Type ImplementationType { get; }

    /// <inheritdoc/>
    public ServiceLifetime Lifetime { get; }
}
